using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Proiect_ASP_final.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string prenume { get; set; }
        public string nume { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext,
            Proiect_ASP_final.Migrations.Configuration>("DefaultConnection"));
        }

        public DbSet<Categorie> Categorii { get; set; }
        public DbSet<Produs> Produse { get; set; }
        public DbSet<CategorieProdus> CategoriiProduse { get; set; }
        public DbSet<ProdusRating> ProduseRatinguri { get; set; }
        public DbSet<Adresa> Adrese { get; set; }
        public DbSet<Comanda> Comenzi { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ProdusComandat> ProduseComandate { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}