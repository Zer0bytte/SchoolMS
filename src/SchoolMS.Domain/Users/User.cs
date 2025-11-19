using SchoolMS.Domain.Attendances;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.StudentClasses;
using SchoolMS.Domain.Submissions;
using SchoolMS.Domain.Users.Enums;
using System.Net.Mail;

namespace SchoolMS.Domain.Users;

public sealed class User : AuditableEntity
{
    private User() { }

    private User(Guid id, string name, string email, string password, Role role) : base(id)
    {
        Name = name;
        Email = email;
        Password = password;
        Role = role;
    }

    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Role Role { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Department> ManagedDepartments { get; set; } = [];
    public ICollection<Class> TaughtClasses { get; set; } = [];
    public ICollection<StudentClass> StudentClasses { get; set; } = [];
    public ICollection<Attendance> Attendances { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];


    public static Result<User> Create(Guid id, string name, string email, string password, Role role)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UserErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return UserErrors.EmailRequired;
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            return UserErrors.Password;
        }

        try
        {
            var emailValidation = new MailAddress(email);
            if (emailValidation.Address != email)
            {
                return UserErrors.InvalidEmail;
            }
        }
        catch (Exception)
        {
            return UserErrors.InvalidEmail;
        }

        return new User(id, name, email, password, role);
    }
}
