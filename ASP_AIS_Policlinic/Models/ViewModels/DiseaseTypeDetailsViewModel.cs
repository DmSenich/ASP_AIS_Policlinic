namespace ASP_AIS_Policlinic.Models.ViewModels
{
    public class DiseaseTypeDetailsViewModel
    {
        public DiseaseType DiseaseType { get; set; }
        public IEnumerable<Disease> Diseases { get; set; }
    }
}
