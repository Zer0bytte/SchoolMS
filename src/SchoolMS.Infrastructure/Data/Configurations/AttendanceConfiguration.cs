using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolMS.Domain.Attendances;

namespace SchoolMS.Infrastructure.Data.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ClassId)
            .IsRequired();

        builder.Property(a => a.StudentId)
            .IsRequired();

        builder.Property(a => a.Date)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.MarkedByTeacherId)
            .IsRequired();



        builder.HasIndex(a => new { a.ClassId, a.StudentId, a.Date });

        builder.HasOne(a => a.Class)
            .WithMany(c => c.Attendances)
            .HasForeignKey(a => a.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Student)
            .WithMany(u => u.Attendances)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.MarkedByTeacher)
            .WithMany()
            .HasForeignKey(a => a.MarkedByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
