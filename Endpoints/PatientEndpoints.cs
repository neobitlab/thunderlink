using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Thunderlink.Data;
using Thunderlink.Models;
using Thunderlink.Validation;

namespace Thunderlink.Endpoints
{
    public static class PatientEndpoints
    {
        public static void MapPatientEndpoints(this WebApplication app)
        {

            app.MapPost("/data/patients", async (ThunderlinkData context, Patient record) =>
            {
                var pass = Garmr.NeoGuard(record);
                if (pass is not Accepted) return pass;
                
                record.PatientID = Garmr.NeoID(record);
                record.Admission = DateTime.Now;

                context.Patient.Add(record);
                await context.SaveChangesAsync();

                return Results.Created($"Patient record created successfully.",
                    new { url = $"/data/patients/{record.PatientID}", record.PatientID });
            });


            app.MapPatch("/data/patients/{id}", async (ThunderlinkData context, string id, Patient record) =>
            {
                var current = await context.Patient.FindAsync(id);
                if (current == null)
                    return Results.NotFound(new { Message = "Patient not found." });

                current.Name = record.Name ?? current.Name;
                current.Room = record.Room ?? current.Room;
                current.Wing = record.Wing ?? current.Wing;
                
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
