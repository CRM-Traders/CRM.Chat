using System.Reflection;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Domain.Common.Entities;
using CRM.Chat.Domain.Entities.ConversationMembers;
using CRM.Chat.Domain.Entities.Conversations;
using CRM.Chat.Domain.Entities.Messages;
using CRM.Chat.Domain.Entities.MessageStatuses;
using CRM.Chat.Domain.Entities.OutboxMessages;
using CRM.Chat.Domain.Entities.UserPresences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CRM.Chat.Persistence.Databases;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUserContext userContext) : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
    public DbSet<MessageStatus> MessageStatuses => Set<MessageStatus>();
    public DbSet<UserPresence> UserPresences => Set<UserPresence>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyAuditInformation()
    {
        var userId = userContext.Id.ToString();
        var userIp = userContext.IpAddress;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreationTracking(userId, userIp);
                    break;

                case EntityState.Modified:
                    entry.Entity.SetModificationTracking(userId, userIp);

                    if (entry.Properties.Any(p => p.Metadata.Name == nameof(AuditableEntity.IsDeleted)) &&
                        entry.Property(nameof(AuditableEntity.IsDeleted)).CurrentValue is true &&
                        entry.Property(nameof(AuditableEntity.IsDeleted)).OriginalValue is false)
                    {
                        entry.Entity.SetDeletionTracking(userId, userIp);
                    }

                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.SetDeletionTracking(userId, userIp);
                    break;
            }
        }
    }
}