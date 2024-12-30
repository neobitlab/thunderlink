using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Thunderlink.Data;
using Thunderlink.Models;

namespace Thunderlink.Endpoints
{
    public static class Garmr
    {
        public static IResult NeoGuard(int age, char gender, int severity, string id)
        {

            switch (age)
            {
                case 0:
                    return Results.BadRequest(new { Message = "Age cannot be zero." });
                case < 0:
                    return Results.BadRequest(new { Message = "Age cannot be a negative integer." });
                case > 122:
                    //based on the oldest person ever whose age has been independently verified, Jeanne Calment (1875–1997)
                    return Results.BadRequest(new { Message = $"Age cannot exceed upper bound (122)" });
                default:
                    break;
            }


            if (gender == '\0') return Results.BadRequest(new { Message = "Gender is a required field." });

            if (!Regex.IsMatch(gender.ToString(), "^[MF]$", RegexOptions.IgnoreCase)) return Results.BadRequest(new { Message = "Gender must be either M or F." });

            switch (severity)
            {
                case 0:
                    return Results.BadRequest(new { Message = "Severity cannot be zero." });
                case < 0:
                    return Results.BadRequest(new { Message = "Severity cannot be a negative integer." });
                case > 5:
                    return Results.BadRequest(new { Message = $"Severity must be a value between 1 and 5." });
                default:
                    break;

            }

            return Results.Created($"/data/patients/{id}", new { Message = "Patient record created successfully.", id });

        }

   
        public static string NeoID(Patient record)
        {

        string UID = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                   .Replace("/", "N")
                   .Replace("+", "E")
                   .Replace("=", "O")
                   .Substring(0, 6)
                   .ToUpper();

            return $"P{record.Age:D2}{(Gender)record.Gender}-{(Severity)record.Severity}-{UID}";
        }
    }
}
