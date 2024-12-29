using System.ComponentModel.DataAnnotations;

namespace Thunderlink.Models
{
    public class Patient
    {
        [Key]
        public string? PatientID { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public char Gender { get; set; }
        [Required]
        public int Severity { get; set; }

        public int Index { get; set; }

        public string? Name { get; set; }

        public string? Room { get; set; }

        public string? Wing { get; set; }

        public DateTime? Admission { get; set; }

        public ICollection<Sensor>? Sensor { get; set; }
        public Station? Station { get; set; }

    }

}
