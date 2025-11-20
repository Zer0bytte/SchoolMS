using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolMS.Domain.Courses;

namespace SchoolMS.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);


        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => new { c.Code, c.DepartmentId })
            .IsUnique();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.DepartmentId)
            .IsRequired();

        builder.Property(c => c.Credits)
            .IsRequired();

        builder.HasOne(c => c.Department)
            .WithMany(d => d.Courses)
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Classes)
            .WithOne(cl => cl.Course)
            .HasForeignKey(cl => cl.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
