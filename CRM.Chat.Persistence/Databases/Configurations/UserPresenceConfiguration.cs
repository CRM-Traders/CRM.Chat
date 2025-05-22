using CRM.Chat.Domain.Entities.UserPresences;
using CRM.Chat.Persistence.Databases.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Chat.Persistence.Databases.Configurations;

public class UserPresenceConfiguration : BaseEntityTypeConfiguration<UserPresence>
{
    public override void Configure(EntityTypeBuilder<UserPresence> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserPresences");

        builder.Property(up => up.UserId)
            .IsRequired();

        builder.Property(up => up.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(up => up.LastActive)
            .IsRequired();

        builder.HasIndex(up => up.UserId)
            .IsUnique();

        builder.HasIndex(up => up.Status);
    }
}