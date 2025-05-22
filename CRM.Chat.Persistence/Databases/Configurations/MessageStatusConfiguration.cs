using CRM.Chat.Domain.Entities.MessageStatuses;
using CRM.Chat.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Chat.Persistence.Databases.Configurations;

public class MessageStatusConfiguration : AuditableEntityTypeConfiguration<MessageStatus>
{
    public override void Configure(EntityTypeBuilder<MessageStatus> builder)
    {
        base.Configure(builder);

        builder.ToTable("MessageStatuses");

        builder.Property(ms => ms.MessageId)
            .IsRequired();

        builder.Property(ms => ms.UserId)
            .IsRequired();

        builder.Property(ms => ms.IsDelivered)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ms => ms.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(ms => new { ms.MessageId, ms.UserId })
            .IsUnique()
            .HasDatabaseName("IX_MessageStatuses_MessageId_UserId");

        builder.HasIndex(ms => new { ms.UserId, ms.IsRead });
        builder.HasIndex(ms => new { ms.MessageId, ms.IsDelivered });
    }
}