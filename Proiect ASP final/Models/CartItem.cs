using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proiect_ASP_final.Models
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int idProdus { get; set; }

        [Required]
        public string idUtilizator { get; set; }

        [Required]
        [Range(1, 999)]
        public int cantitate { get; set; }

        public string denumireProdus { get; set; }
        public int cantitateMaxima { get; set; }

        public virtual Produs produs { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}