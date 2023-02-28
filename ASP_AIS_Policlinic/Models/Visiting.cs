using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_AIS_Policlinic.Models
{
    public class Visiting
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [ForeignKey("Doctor")]
        [Required(ErrorMessage = "Выберите доктора")]
        [DisplayName("Доктор")]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        [DisplayName("Доктор")]
        public Doctor? Doctor { get; set; }

        [ForeignKey("Patient")]
        [Required(ErrorMessage = "Выберите пациента")]
        [DisplayName("Пациент")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        [DisplayName("Пациент")]
        public Patient? Patient { get; set; }

        [Required(ErrorMessage = "Выберите дату визита")]
        [DataType(DataType.Date)]
        [DisplayName("Дата визита")]
        public DateTime DateVisiting { get; set; }

        public ICollection<Disease> Diseases { get; set; }

        public Visiting()
        {
            Diseases = new List<Disease>();
        }

    }
}
