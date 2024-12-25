using Microsoft.EntityFrameworkCore;
using Thunderlink.Data;
using Thunderlink.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("garmr")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ThunderlinkData>(options =>
    options.UseMySQL(connectionString));


var app = builder.Build();

app.MapPatientEndpoints();
app.MapSensorEndpoints();
app.MapStationEndpoints();

app.MapGet("/status", () =>
{
    return Results.Ok(new
    {
        status = "Online",
        timestamp = DateTime.UtcNow,
        version = "0.0.1-dev"
    });
});

app.Run();
