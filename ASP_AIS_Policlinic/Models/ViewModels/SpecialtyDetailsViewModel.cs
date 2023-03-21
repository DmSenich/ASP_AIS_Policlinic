namespace ASP_AIS_Policlinic.Models.ViewModels
{
    public class SpecialtyDetailsViewModel
    {
        public Specialty Specialty { get; set; }
        public IEnumerable<Doctor> Doctors { get; set; }    
    }
}
