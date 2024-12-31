using Microsoft.EntityFrameworkCore;
using Thunderlink.Data;
using Thunderlink.Models;
namespace Thunderlink.Endpoints
{
    public static class StationEndpoints
    {
        public static void MapStationEndpoints(this WebApplication app)
        {

            app.MapPost("/data/stations", async (ThunderlinkData context, Station unit) =>
            {
                if (string.IsNullOrWhiteSpace(unit.StationID))
                    return Results.BadRequest(new { Message = "StationID field is required." });

                unit.Timestamp = DateTime.Now;

                context.Station.Add(unit);
                await context.SaveChangesAsync();

                return Results.Created($"/data/stations/{unit.StationID}", new { Message = "Station entry created successfully.", Station = unit });
            });

            app.MapPatch("/data/stations/{id}", async (ThunderlinkData context, string id, Station unit) =>
            {
                var current = await context.Station.FindAsync(id);
                if (current == null)
                    return Results.NotFound(new { Message = "Station not found." });

                current.Room = unit.Room ?? current.Room;
                current.Wing = unit.Wing ?? current.Wing;
                current.Status = unit.Status ?? current.Status;
                current.PatientID = unit.PatientID ?? current.PatientID;

                await context.SaveChangesAsync();
                return Results.Ok(new { Message = "Station entry updated successfully." });
            });

            app.MapGet("/data/stations", async (ThunderlinkData context, int? page, int? size) =>
            {
                var units = await context.Station
                    .AsNoTracking()
                    .OrderBy(s => s.StationID)
                    .Skip((page.GetValueOrDefault(1) - 1) * size.GetValueOrDefault(10))
                    .Take(size.GetValueOrDefault(10))
                    .ToListAsync();

                return Results.Ok(units);
            });

            app.MapGet("/data/stations/{id}", async (ThunderlinkData context, string id) =>
            {
                var unit = await context.Station.FindAsync(id);
                if (unit == null)
                    return Results.NotFound(new { Message = "Station unit not found." });

                return Results.Ok(unit);
            });

            app.MapDelete("/data/stations/{id}", async (ThunderlinkData context, string id) =>
            {
                var unit = await context.Station.FindAsync(id);
                if (unit == null)
                    return Results.NotFound(new { Message = "Station unit not found." });

                context.Station.Remove(unit);
                await context.SaveChangesAsync();
                return Results.NoContent();
            });

        }
    }
}
