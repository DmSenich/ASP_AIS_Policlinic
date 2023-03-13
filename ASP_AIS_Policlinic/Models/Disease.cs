using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_AIS_Policlinic.Models
{
    public class Disease
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Введите описание диагноза")]
        [DisplayName("Описание")]
        public string Description { get; set; }

        [ForeignKey("DiseaseType")]
        [DisplayName("Тип заболевания")]
        [Required(ErrorMessage = "Выберите диагноз")]
        public int DiseaseTypeId { get; set; }

        [ForeignKey("DiseaseTypeId")]
        [DisplayName("Тип заболевания")]
        public DiseaseType? DiseaseType { get; set; }

        [ForeignKey("Visiting")]
        [DisplayName("Визит")]
        public int VisitingId { get; set; }

        [ForeignKey("VisitingId")]
        [DisplayName("Визит")]
        public Visiting? Visiting { get; set; }
    }
}
