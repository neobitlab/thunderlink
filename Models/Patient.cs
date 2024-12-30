using System.ComponentModel.DataAnnotations;

namespace Thunderlink.Models
{
    public class Patient
    {
        [Key]
        public string? PatientID { get; set; }
        public int Age { get; set; }
        public char Gender { get; set; }
        public int Severity { get; set; }

        public string? Name { get; set; }

        public string? Room { get; set; }

        public string? Wing { get; set; }

        public DateTime? Admission { get; set; }

        public ICollection<Sensor>? Sensor { get; set; }
        public Station? Station { get; set; }

    }
    public enum Severity { T = 1, M = 2, G = 3, S = 4, C = 5 };
    public enum Gender { XX = 'F', XY = 'M' };
}
