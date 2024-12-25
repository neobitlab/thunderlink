using Microsoft.EntityFrameworkCore;
using Thunderlink.Models;

namespace Thunderlink.Data
{
    public class ThunderlinkData(DbContextOptions<ThunderlinkData> options) : DbContext(options)
    {
        public DbSet<Patient> Patient { get; set; }
        public DbSet<Station> Station { get; set; }
        public DbSet<Sensor> Sensor { get; set; }
    }
}
