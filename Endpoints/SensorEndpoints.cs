using Microsoft.EntityFrameworkCore;
using Thunderlink.Data;
using Thunderlink.Models;
namespace Thunderlink.Endpoints
{
    public static class SensorEndpoints
    {
        public static void MapSensorEndpoints(this WebApplication app)
        {
            app.MapPost("/data/sensors", async (ThunderlinkData context, Sensor unit) =>
            {
                if (string.IsNullOrWhiteSpace(unit.SensorID))
                    return Results.BadRequest(new { Message = "SensorID field is required." });

                context.Sensor.Add(unit);
                await context.SaveChangesAsync();

                return Results.Created($"/data/sensors/{unit.SensorID}", new { Message = "Sensor entry created successfully.", Sensor = unit });
            });

            app.MapPatch("/data/sensors/{id}", async (ThunderlinkData context, string id, Sensor unit) =>
            {
                var current = await context.Sensor.FindAsync(id);
                if (current == null)
                    return Results.NotFound(new { Message = "Sensor not found." });

                current.SensorType = unit.SensorType ?? current.SensorType;
                current.SensorData = unit.SensorData ?? current.SensorData;
                current.Status = unit.Status ?? current.Status;
                current.PatientID = unit.PatientID ?? current.PatientID;
                current.StationID = unit.StationID ?? current.StationID;

                await context.SaveChangesAsync();
                return Results.Ok(new { Message = "Sensor entry updated successfully." });
            });

            app.MapGet("/data/sensors", async (ThunderlinkData context, int? page, int? size) =>
            {
                var units = await context.Sensor
                    .AsNoTracking()
                    .Skip((page.GetValueOrDefault(1) - 1) * size.GetValueOrDefault(10))
                    .Take(size.GetValueOrDefault(10))
                    .ToListAsync();

                return Results.Ok(units);
            });

            app.MapGet("/data/sensor/{id}", async (ThunderlinkData context, string id) =>
            {
                var unit = await context.Sensor.FindAsync(id);
                if (unit == null)
                    return Results.NotFound(new { Message = "Sensor unit not found." });

                return Results.Ok(unit);
            });

            app.MapDelete("/data/sensors/{id}", async (ThunderlinkData context, string id) =>
            {
                var unit = await context.Sensor.FindAsync(id);
                if (unit == null)
                    return Results.NotFound(new { Message = "Sensor unit not found." });

                context.Sensor.Remove(unit);
                await context.SaveChangesAsync();
                return Results.NoContent();
            });

        }
    }
}
