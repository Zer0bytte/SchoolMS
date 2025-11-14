using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Attendances;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Notifications;
using SchoolMS.Domain.StudentClasses;
using SchoolMS.Domain.Submissions;
using SchoolMS.Domain.Users;

namespace SchoolMS.Application.Common.Interfaces;

public interface IAppDbContext
{
    public DbSet<User> Users { get; }
    public DbSet<Assignment> Assignments { get; }
    public DbSet<Attendance> Attendances { get; }
    public DbSet<Class> Classes { get; }
    public DbSet<Course> Courses { get; }
    public DbSet<Department> Departments { get; }
    public DbSet<Notification> Notifications { get; }
    public DbSet<StudentClass> StudentClasses { get; }
    public DbSet<Submission> Submissions { get; }
    public DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
