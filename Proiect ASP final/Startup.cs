using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Proiect_ASP_final.Models;

[assembly: OwinStartupAttribute(typeof(Proiect_ASP_final.Startup))]
namespace Proiect_ASP_final
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            InitAppRoles();
        }

        private void InitAppRoles()
        {

            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Admin"))
            {

                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                var adminUser = new ApplicationUser();
                adminUser.UserName = "admin@test.here";
                adminUser.Email = "admin@test.here";

                var createdAdmin = userManager.Create(adminUser, "Test0/");
                if (createdAdmin.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
            }

            if (!roleManager.RoleExists("Seller"))
            {

                var role = new IdentityRole();
                role.Name = "Seller";
                roleManager.Create(role);

                var sellerUser = new ApplicationUser();
                sellerUser.UserName = "seller@test.here";
                sellerUser.Email = "seller@test.here";

                var createdSeller = userManager.Create(sellerUser, "Test0/");
                if (createdSeller.Succeeded)
                {
                    userManager.AddToRole(sellerUser.Id, "Seller");
                }
            }

            if (!roleManager.RoleExists("User"))
            {

                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "user@test.here";
                user.Email = "user@test.here";

                var createdUser = userManager.Create(user, "Test0/");
                if (createdUser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "User");
                }
            }
        }
    }
}
