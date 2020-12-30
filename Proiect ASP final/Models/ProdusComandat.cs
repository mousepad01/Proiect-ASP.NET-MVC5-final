using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proiect_ASP_final.Models
{
    [Table("ProduseComandate")]
    public class ProdusComandat
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int idProdus { get; set; }
        public string denumireProdus { get; set; }

        [Required]
        public int idComanda { get; set; }

        [Required]
        [Range(1, 999)]
        public int cantitate { get; set; }

        public virtual Produs produs { get; set; }
        public virtual Comanda comanda { get; set; }
    }
}