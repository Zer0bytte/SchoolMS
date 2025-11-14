using EedadyAbofam.Infrastructure.Data;
using SchoolMS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.InitialiseDatabaseAsync();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();