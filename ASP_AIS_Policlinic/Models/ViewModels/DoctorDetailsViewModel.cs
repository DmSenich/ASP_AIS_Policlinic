namespace ASP_AIS_Policlinic.Models.ViewModels
{
    public class DoctorDetailsViewModel
    {
        public Doctor Doctor { get; set; }
        public IEnumerable<Specialty> Specialties { get; set; }
    }
}
