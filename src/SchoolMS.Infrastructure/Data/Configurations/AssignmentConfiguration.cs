using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolMS.Domain.Assignments;

namespace SchoolMS.Infrastructure.Data.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ClassId)
            .IsRequired();

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(2000);

        builder.Property(a => a.DueDate)
            .IsRequired();


        builder.Property(a => a.CreatedByTeacherId)
            .IsRequired();

        builder.HasOne(a => a.Class)
            .WithMany(c => c.Assignments)
            .HasForeignKey(a => a.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.CreatedByTeacher)
            .WithMany()
            .HasForeignKey(a => a.CreatedByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Submissions)
            .WithOne(s => s.Assignment)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
