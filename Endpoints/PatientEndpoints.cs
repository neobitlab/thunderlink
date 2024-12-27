using Microsoft.EntityFrameworkCore;
using Thunderlink.Data;
using Thunderlink.Models;

namespace Thunderlink.Endpoints
{
    public static class PatientEndpoints
    {
        public static void MapPatientEndpoints(this WebApplication app)
        {

            app.MapPost("/data/patients", async (ThunderlinkData context, Patient record) =>
            {
                if (string.IsNullOrWhiteSpace(record.PatientID))
                    return Results.BadRequest(new { Message = "PatientID field is required." });

                context.Patient.Add(record);
                await context.SaveChangesAsync();

                return Results.Created($"/data/patients/{record.PatientID}", new { Message = "Patient record created successfully." });
            });

            app.MapPatch("/data/patients/{id}", async (ThunderlinkData context, string id, Patient record) =>
            {
                var current = await context.Patient.FindAsync(id);
                if (current == null)
                    return Results.NotFound(new { Message = "Sensor not found." });

                current.Name = record.Name ?? current.Name;
                current.Birthdate = record.Birthdate ?? current.Birthdate;
                current.Room = record.Room ?? current.Room;
                current.Wing = record.Wing ?? current.Wing;
                current.Admission = record.Admission ?? current.Admission;

                await context.SaveChangesAsync();
                return Results.Ok(new { Message = "Patient record updated successfully." });
            });

            app.MapGet("/data/patients", async (ThunderlinkData context, int? page, int? size) =>
            {
                var records = await context.Patient
                    .AsNoTracking()
                    .Include(p => p.Sensor)
                    .OrderBy(p => p.PatientID)
                    .Skip((page.GetValueOrDefault(1) - 1) * size.GetValueOrDefault(10))
                    .Take(size.GetValueOrDefault(10))
                    .ToListAsync();

                return Results.Ok(records);
            });

            app.MapGet("/data/patients/{id}", async (ThunderlinkData context, string id) =>
            {
                var record = await context.Patient
                    .AsNoTracking()
                    .Include(p => p.Sensor)
                    .Include(p => p.Station)
                    .Select(p => new
                    {
                        p.PatientID,
                        p.Name,
                        p.Birthdate,
                        p.Room,
                        p.Wing,
                        p.Admission,
                        Sensor = p.Sensor.Select(s => new
                        {
                            s.SensorID,
                            s.SensorType,
                            s.SensorData,
                            s.Status
                        }),
                        Station = new
                        {
                            p.Station.StationID,
                            p.Station.Status
                        }
                    })
                    .FirstOrDefaultAsync(p => p.PatientID == id);

                if (record == null)
                    return Results.NotFound(new { Message = "Patient record not found." });

                return Results.Ok(record);
            });

            app.MapDelete("/data/patients/{id}", async (ThunderlinkData context, string id) =>
            {
                var record = await context.Patient.FindAsync(id);
                if (record == null)
                    return Results.NotFound(new { Message = "Sensor record not found." });

                context.Patient.Remove(record);
                await context.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
