using ASP_AIS_Policlinic.Areas.Identity.Data;

namespace ASP_AIS_Policlinic.Models.ViewModels
{
    public class UserDetailsViewModel
    {
        public PoliclinicUser User { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}
