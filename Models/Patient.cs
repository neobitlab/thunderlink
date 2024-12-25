using System.ComponentModel.DataAnnotations;

namespace Thunderlink.Models
{
    public class Patient
    {
        [Key]
        public required string PatientID { get; set; }

        public string? Name { get; set; }

        public DateTime? Birthdate { get; set; }

        public string? Room { get; set; }

        public string? Wing { get; set; }

        public DateTime? Admission { get; set; }

        public ICollection<Sensor>? Sensor { get; set; }
        public Station? Station { get; set; }
    }
}
