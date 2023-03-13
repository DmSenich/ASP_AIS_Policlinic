using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASP_AIS_Policlinic.Models
{
    public class Specialty
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "Введите название специальности")]
        [DisplayName("Название специальности")]
        public string NameSpecialty { get; set; }

        public ICollection<Doctor>? Doctors { get; set; }
        public Specialty()
        {
            Doctors = new List<Doctor>();
        }
    }
}
