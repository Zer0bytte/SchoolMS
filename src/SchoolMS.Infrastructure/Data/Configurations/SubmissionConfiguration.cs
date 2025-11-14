using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolMS.Domain.Submissions;

namespace SchoolMS.Infrastructure.Data.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.AssignmentId)
            .IsRequired();

        builder.Property(s => s.StudentId)
            .IsRequired();


        builder.Property(s => s.FileUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Grade)
            .HasPrecision(5, 2);

        builder.Property(s => s.Remarks)
            .HasMaxLength(1000);


        builder.HasIndex(s => new { s.AssignmentId, s.StudentId })
            .IsUnique();

        builder.HasOne(s => s.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Student)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.GradedByTeacher)
            .WithMany()
            .HasForeignKey(s => s.GradedByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
