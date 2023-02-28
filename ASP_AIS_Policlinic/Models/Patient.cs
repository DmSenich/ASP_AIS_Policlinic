using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASP_AIS_Policlinic.Models
{
    public class Patient
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

        [MaxLength(15)]
        [Required(ErrorMessage = "Введите название области")]
        [DisplayName("Область")]
        public string Area { get; set; }

        [MaxLength(15)]
        [Required(ErrorMessage = "Введите название города")]
        [DisplayName("Город")]
        public string City { get; set; }

        [MaxLength(4)]
        [Required(ErrorMessage = "Введите номер дома")]
        [DisplayName("Дом")]
        public string House { get; set; }

        [DisplayName("Квартира")]
        public int? Apartment { get; set; }

        [Required(ErrorMessage = "Введите дату рождения")]
        [DataType(DataType.Date)]
        [DisplayName("Дата рождения")]
        public DateTime DateBirth { get; set; }

        public ICollection<Visiting> Visitings { get; set; }
        public Patient()
        {
            Visitings = new List<Visiting>();
        }
    }
}
