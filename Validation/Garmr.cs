using Thunderlink.Models;

namespace Thunderlink.Validation
{
    public static class Garmr
    {
        public static IResult NeoGuard(Patient record)
        {

            // Validate age
            if (record.Age == 0)
                return Results.BadRequest(new { Message = "Age cannot be zero and is required." });

            if (record.Age < 0)
                return Results.BadRequest(new { Message = "Age cannot be a negative integer." });

            if (record.Age > 122)
                return Results.BadRequest(new { Message = "Age cannot exceed upper bound (122)." });

            // Validate gender
            if (record.Gender == '\0')
                return Results.BadRequest(new { Message = "Gender is required." });

            if (!"MF".Contains(char.ToUpper(record.Gender)))
                return Results.BadRequest(new { Message = "Gender must be either M or F." });

            // Validate severity

            if (record.Severity == 0)
                return Results.BadRequest(new { Message = "Severity cannot be zero and is required." });

            if (record.Severity < 0)
                return Results.BadRequest(new { Message = "Severity cannot be a negative integer." });

            if (record.Severity > 5)
                return Results.BadRequest(new { Message = "Severity must be a value between 1 and 5." });

            return Results.Accepted();
        }


        public static string NeoID(Patient record)
        {

            //Generate unique identifier
            string UID = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                       .Replace("/", "N")
                       .Replace("+", "E")
                       .Replace("=", "O")
                       .Substring(0, 8)
                       .ToUpper();

            //Generate PatientID
            return $"P{record.Age:D2}{(Gender)record.Gender}-{(Severity)record.Severity}-{UID}";
        }
    }
}
