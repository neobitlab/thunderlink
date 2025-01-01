using System.ComponentModel.DataAnnotations;

namespace Thunderlink.Models
{
    public class Patient
    {
        [Key]
        [MaxLength(16)]
        public string? PatientID { get; set; }
        public int Age { get; set; }
        public char Gender { get; set; }
        public int Severity { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }
        
        [MaxLength(5)]
        public string? Room { get; set; }
        
        [MaxLength(5)]
        public string? Wing { get; set; }

        public DateTime? Admission { get; set; }

        public ICollection<Sensor>? Sensor { get; set; }
        public Station? Station { get; set; }

    }
    public enum Severity { T = 1, M = 2, G = 3, S = 4, C = 5 };
    public enum Gender { XX = 'F', XY = 'M' };
}
