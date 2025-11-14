using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Attendances;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Notifications;
using SchoolMS.Domain.StudentClasses;
using SchoolMS.Domain.Submissions;
using SchoolMS.Domain.Users;


namespace SchoolMS.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<StudentClass> StudentClasses => Set<StudentClass>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
