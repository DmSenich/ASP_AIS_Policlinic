using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASP_AIS_Policlinic.Models
{
    public class DiseaseType
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [MaxLength(15)]
        [Required(ErrorMessage = "Введите название диагноза")]
        [DisplayName("Тип заболевания")]
        public string NameDisease { get; set; }

        public ICollection<Disease>? Diseases { get; set; }

        public DiseaseType()
        {
            Diseases = new List<Disease>();
        }
    }
}
