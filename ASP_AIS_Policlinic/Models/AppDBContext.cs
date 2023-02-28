using Microsoft.EntityFrameworkCore;
using ASP_AIS_Policlinic.Models;

namespace ASP_AIS_Policlinic.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        {

        }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Visiting> Visitings { get; set; }
        public DbSet<DiseaseType> DiseaseTypes { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Specialty> Specialties { get; set; }

    }
}
