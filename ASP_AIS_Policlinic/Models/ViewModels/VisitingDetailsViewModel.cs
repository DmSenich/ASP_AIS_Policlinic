namespace ASP_AIS_Policlinic.Models.ViewModels
{
    public class VisitingDetailsViewModel
    {
        public Visiting Visiting { get; set; }
        public IEnumerable<Disease> Diseases { get; set; }
    }
}
