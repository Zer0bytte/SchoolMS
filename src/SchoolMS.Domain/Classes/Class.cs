using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Attendances;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.StudentClasses;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Classes;

public sealed class Class : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public Guid CourseId { get; private set; }
    public Guid TeacherId { get; private set; }
    public string Semester { get; private set; } = default!;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Course Course { get; private set; } = default!;
    public User Teacher { get; private set; } = default!;
    public ICollection<StudentClass> StudentClasses { get; private set; } = [];
    public ICollection<Attendance> Attendances { get; private set; } = [];
    public ICollection<Assignment> Assignments { get; private set; } = [];



    public Class(Guid id, string name, Guid courseId, Guid teacherId, string semester, DateOnly startDate, DateOnly endDate) : base(id)
    {
        Name = name;
        CourseId = courseId;
        TeacherId = teacherId;
        Semester = semester;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Result<Class> Create(Guid id, string name, Guid courseId, Guid teacherId, string semester, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ClassErrors.NameRequired;

        if (courseId == Guid.Empty)
            return ClassErrors.CourseIdInvalid;

        if (teacherId == Guid.Empty)
            return ClassErrors.TeacherIdInvalid;

        if (string.IsNullOrWhiteSpace(semester))
            return ClassErrors.SemesterRequired;

        if (startDate == default)
            return ClassErrors.StartDateInvalid;

        if (endDate == default)
            return ClassErrors.EndDateInvalid;

        if (endDate < startDate)
            return ClassErrors.EndDateBeforeStartDate;

        return new Class(id, name, courseId, teacherId, semester, startDate, endDate);
    }

    public Result<Class> Update(string? name, string? semester, DateOnly? startDate, DateOnly? endDate)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (!string.IsNullOrWhiteSpace(semester))
            Semester = semester;

        if (startDate.HasValue)
            StartDate = startDate.Value;

        if (endDate.HasValue)
            EndDate = endDate.Value;

        if (endDate < startDate)
            return ClassErrors.EndDateBeforeStartDate;



        return this;
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
    }
}