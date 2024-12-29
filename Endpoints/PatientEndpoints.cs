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
                

                using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    int nextIndex = await context.Patient.MaxAsync(p => (int?)p.Index) ?? 0;
                    record.Index = nextIndex + 1;

                    record.PatientID = $"P{record.Age:D2}{record.Gender}{record.Severity}-{record.Index:D4}";
                    record.Admission = DateTime.Now;

                    context.Patient.Add(record);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Garmr.NeoGuard(record.Age, record.Gender, record.Severity, record.PatientID);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Results.Json(new { Message = "An error occurred while processing the request.", Error = ex.Message }, statusCode: 500);
                }
            });


            app.MapPatch("/data/patients/{id}", async (ThunderlinkData context, string id, Patient record) =>
            {
                var current = await context.Patient.FindAsync(id);
                if (current == null)
                    return Results.NotFound(new { Message = "Sensor not found." });

                current.Name = record.Name ?? current.Name;
                current.Age = record.Age != 0 ? record.Age : current.Age;
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
                        p.Age,
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
