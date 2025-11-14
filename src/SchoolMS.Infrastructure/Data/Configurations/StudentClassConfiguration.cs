using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolMS.Domain.StudentClasses;

namespace SchoolMS.Infrastructure.Data.Configurations;

public class StudentClassConfiguration : IEntityTypeConfiguration<StudentClass>
{
    public void Configure(EntityTypeBuilder<StudentClass> builder)
    {
        builder.ToTable("StudentClasses");

        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.StudentId)
            .IsRequired();

        builder.Property(sc => sc.ClassId)
            .IsRequired();



        builder.HasIndex(sc => new { sc.StudentId, sc.ClassId })
            .IsUnique();

        builder.HasOne(sc => sc.Student)
            .WithMany(u => u.StudentClasses)
            .HasForeignKey(sc => sc.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sc => sc.Class)
            .WithMany(c => c.StudentClasses)
            .HasForeignKey(sc => sc.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
