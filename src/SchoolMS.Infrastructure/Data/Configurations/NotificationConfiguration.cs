using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolMS.Domain.Notifications;

namespace SchoolMS.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.RecipientRole)
            .HasMaxLength(20);


        builder.Property(n => n.IsRead)
            .HasDefaultValue(false);

        builder.HasIndex(n => new { n.RecipientRole, n.RecipientId, n.IsRead });

        builder.HasOne(n => n.Recipient)
            .WithMany()
            .HasForeignKey(n => n.RecipientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
