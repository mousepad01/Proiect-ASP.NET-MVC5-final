using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_ASP_final.Models
{

    [Table("Adrese")]
    public class Adresa
    {
        [Key]
        public int idAdresa { get; set; }

        [Required(ErrorMessage = "Tara este obligatorie!")]
        [MinLength(3, ErrorMessage = "Tara trebuie să conțină minim trei caractere!")]
        [MaxLength(25, ErrorMessage = "Tara poate să conțină maxim 25 de caractere!")]
        public string tara { get; set; }

        [Required(ErrorMessage = "Orasul este obligatoriu!")]
        [MinLength(3, ErrorMessage = "Orasul trebuie să conțină minim trei caractere!")]
        [MaxLength(100, ErrorMessage = "Orasul poate să conțină maxim 100 de caractere!")]
        public string oras { get; set; }

        [Required(ErrorMessage = "Strada este obligatorie!")]
        [MinLength(3, ErrorMessage = "Strada trebuie să conțină minim trei caractere!")]
        [MaxLength(100, ErrorMessage = "Strada poate să conțină maxim 100 de caractere!")]
        public string strada { get; set; }

        [Required(ErrorMessage = "Nr. străzii este obligatoriu!")]
        [Range(0, 100, ErrorMessage = "Nr. străzii trebuie să fie cuprins între 0 și 100!")]
        public int nr { get; set; }

        [Required(ErrorMessage = "Identificatorul blocului este obligatoriu! Poate fi înlocuit cu nr. casei.")]
        [MaxLength(100, ErrorMessage = "Identificatorul blocului / nr. casei nu poate depăși 100 de caractere.")]
        public string bloc { get; set; }

        public string scara { get; set; }
        public string etaj { get; set; }
        public string apartament { get; set; }

        public string idUtilizator { get; set; }

        public string mentiune { get; set; }

    }
}