using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP_final.Models
{
    [Table("Comenzi")]
    public class Comanda
    {
        [Key]
        public int idComanda { get; set; }

        [Required]
        public DateTime dataPlasare { get; set; }

        [Required]
        public int idAdresa { get; set; }

        public string idUtilizator { get; set; }

        [Required]
        public int sumaDePlata { get; set; }

        public DateTime dataFinalizare { get; set; }

        public virtual Adresa Adresa { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual IEnumerable<SelectListItem> Adrese { get; set; }
        public virtual ICollection<Produs> ProduseComandate { get; set; }

    }
}