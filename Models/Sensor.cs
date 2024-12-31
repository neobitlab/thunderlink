using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Thunderlink.Models
{
    public class Sensor
    {
        [Key]
        public required string SensorID { get; set; }

        public string? SensorType { get; set; }

        public string? SensorData { get; set; }

        public string? Status { get; set; }
        [ForeignKey("Patient")]
        public string? PatientID { get; set; }

        [ForeignKey("Station")]
        public string? StationID { get; set; }
    }
}
