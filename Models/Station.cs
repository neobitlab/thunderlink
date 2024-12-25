using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Thunderlink.Models
{
    public class Station
    {
        [Key]
        public required string StationID { get; set; }

        public string? Room { get; set; }
        public string? Wing { get; set; }

        public string? Status { get; set; }

        public DateTime? Timestamp { get; set; }

        [ForeignKey("Patient")]
        public string? PatientID { get; set; }
        
        [JsonIgnore]
        public Patient? Patient { get; set; }
    }
}
