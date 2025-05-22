using CRM.Chat.Domain.Entities.Messages;
using CRM.Chat.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Chat.Persistence.Databases.Configurations;

public class MessageConfiguration : AuditableEntityTypeConfiguration<Message>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder.ToTable("Messages");

        builder.Property(m => m.ConversationId)
            .IsRequired();

        builder.Property(m => m.SenderId)
            .IsRequired();

        builder.Property(m => m.Content)
            .IsRequired();

        builder.Property(m => m.SentAt)
            .IsRequired();

        builder.Property(m => m.IsEdited)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(m => new { m.ConversationId, m.SentAt });
        builder.HasIndex(m => m.SenderId);

        // Configure relationships
        builder.HasMany(m => m.Attachments)
            .WithOne()
            .HasForeignKey(a => a.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Statuses)
            .WithOne()
            .HasForeignKey(s => s.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}