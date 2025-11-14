using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Infrastructure.Data.Configurations;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("Classes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CourseId)
            .IsRequired();

        builder.Property(c => c.TeacherId)
            .IsRequired();

        builder.Property(c => c.Semester)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);


        builder.HasOne(c => c.Course)
            .WithMany(co => co.Classes)
            .HasForeignKey(c => c.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Teacher)
            .WithMany(u => u.TaughtClasses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.StudentClasses)
            .WithOne(sc => sc.Class)
            .HasForeignKey(sc => sc.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Attendances)
            .WithOne(a => a.Class)
            .HasForeignKey(a => a.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Assignments)
            .WithOne(a => a.Class)
            .HasForeignKey(a => a.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
