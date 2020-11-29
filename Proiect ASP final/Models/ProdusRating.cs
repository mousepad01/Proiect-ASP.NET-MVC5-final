using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proiect_ASP_final.Models
{
    [Table("ProduseRatinguri")]
    public class ProdusRating
    {
        [Key]
        public int prodRating { get; set; }

        [Required]
        public int idProdus { get; set; }

        public string idUtilizator { get; set; }
        public string numeUtilizator { get; set; }

        public DateTime dataReview { get; set; }

        [Required]
        [Range(1, 5)]
        public int rating { get; set; }

        [MaxLength(1024)]
        public string descriere { get; set; }
       
        public virtual Produs produs { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}