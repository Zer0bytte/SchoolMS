using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;
using System;

namespace SchoolMS.Infrastructure.Data;

public class AppDbContextInitializer(AppDbContext context, ILogger<AppDbContextInitializer> logger)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        if (!context.Users.Any())
        {

            var teacher = User.Create(Guid.CreateVersion7(), "Teacher", "teacher@email.com", "Password@123", Role.Teacher).Value;
            var student = User.Create(Guid.CreateVersion7(), "Student", "student@email.com", "Password@123", Role.Student).Value;
            var admin = User.Create(Guid.CreateVersion7(), "Admin", "admin@email.com", "Password@123", Role.Admin).Value;

            context.Users.Add(teacher);
            context.Users.Add(student);
            context.Users.Add(admin);
            await context.SaveChangesAsync();
            var department = Department.Create(Guid.CreateVersion7(), "Default Department", "Description of default department", teacher.Id).Value;
            context.Departments.Add(department);
            await context.SaveChangesAsync();
        }


    }
}
public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}