using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASP_AIS_Policlinic.Models
{
    public class Doctor
    {
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [MaxLength(15)]
        [Required(ErrorMessage = "Введите фамилию")]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }

        [MaxLength(15)]
        [Required(ErrorMessage = "Введите имя")]
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        [MaxLength(15)]
        [DisplayName("Отчество")]
        public string? Patronymic { get; set; }

        [Required(ErrorMessage = "Укажите опыт работы (0, если отсутствует)")]
        [DisplayName("Опыт работы (в годах)")]
        public int WorkExperience { get; set; }

        [DisplayName("Фото")]
        public string? PathPhoto { get; set; }

        public ICollection<Specialty> Specialties { get; set; }
        public ICollection<Visiting> Visitings { get; set; }
        public Doctor()
        {
            Visitings = new List<Visiting>();
            Specialties = new List<Specialty>();
        }
    }
}
