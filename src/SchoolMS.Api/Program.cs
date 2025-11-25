using Asp.Versioning.Conventions;
using Scalar.AspNetCore;
using SchoolMS.Api;
using SchoolMS.Api.Endpoints;
using SchoolMS.Application;
using SchoolMS.Infrastructure;
using SchoolMS.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();
app.UseStaticFiles();
await app.InitialiseDatabaseAsync();


//if (app.Environment.IsDevelopment())
//{
//}

// I Will Leave it for testing the submission purpose

app.MapOpenApi();
app.MapScalarApiReference();

app.UseCoreMiddlewares(builder.Configuration);

var vset = app.NewApiVersionSet()
    .HasApiVersion(1.0)
    .ReportApiVersions()
    .Build();

app.MapIdentityEndpoints(vset);
app.MapDepartmentEndpoints();
app.MapCourseEndpoints();
app.MapTeacherClassEndpoints();
app.MapTeacherAttendanceEndpoints();
app.MapTeacherassignmentEndpoints();
app.MapStudentClassesEndpoints();
app.MapStudentEndpoints();
app.MapStudentAssignmentEndpoints();
app.MapNotificationEndpoints();
app.Run();