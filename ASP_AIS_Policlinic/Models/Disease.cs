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
    }
}
