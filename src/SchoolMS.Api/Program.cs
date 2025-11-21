using Scalar.AspNetCore;
using SchoolMS.Api;
using SchoolMS.Api.Endpoints;
using SchoolMS.Application;
using SchoolMS.Infrastructure;
using SchoolMS.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);



var app = builder.Build();

await app.InitialiseDatabaseAsync();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseCoreMiddlewares(builder.Configuration);


app.MapIdentityEndpoints();
app.MapDepartmentEndpoints();
app.MapCourseEndpoints();
app.MapTeacherClassEndpoints();
app.Run();