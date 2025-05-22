using CRM.Chat.Domain.Entities.ConversationMembers;
using CRM.Chat.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Chat.Persistence.Databases.Configurations;

public class ConversationMemberConfiguration : AuditableEntityTypeConfiguration<ConversationMember>
{
    public override void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        base.Configure(builder);

        builder.ToTable("ConversationMembers");

        builder.Property(cm => cm.ConversationId)
            .IsRequired();

        builder.Property(cm => cm.UserId)
            .IsRequired();

        builder.Property(cm => cm.IsAdmin)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(cm => cm.JoinedAt)
            .IsRequired();

        builder.HasIndex(cm => new { cm.ConversationId, cm.UserId })
            .IsUnique()
            .HasDatabaseName("IX_ConversationMembers_ConversationId_UserId");

        builder.HasIndex(cm => cm.UserId);
    }
}