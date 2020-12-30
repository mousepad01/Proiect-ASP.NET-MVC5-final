using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Proiect_ASP_final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Proiect_ASP_final.Controllers
{
    [RequireHttps]
    public class ProfileController : Controller
    {
        private ApplicationUserManager _userManager;

        private Models.ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var useri = (from u in db.Users
                         select u).ToList<ApplicationUser>();

            List<bool> eBlocat = new List<bool>();
            foreach (var u in useri)
            {
                ApplicationUser user = db.Users.Find(u.Id);
                if (UserManager.IsInRole(user.Id, "BlockedUser") || UserManager.IsInRole(user.Id, "BlockedSeller"))
                {
                    eBlocat.Add(true);
                }
                else
                {
                    eBlocat.Add(false);
                }
            }

            ViewBag.eBlocat = eBlocat;
            ViewBag.useri = useri;

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        [Authorize(Roles = "User,Seller,Admin")]
        public ActionResult MyProfile()
        {
            String userId = User.Identity.GetUserId();
            var useri = (from u in db.Users
                         where u.Id == userId
                         select u).ToList<ApplicationUser>();

            ViewBag.user = useri[0];
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ViewProfile(String userId)
        {
            System.Diagnostics.Debug.WriteLine(userId);
            var useri = (from u in db.Users
                         where u.Id == userId
                         select u).ToList<ApplicationUser>();

            ViewBag.user = useri[0];
            return View("MyProfile");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BanUser(String userId) 
        {
            System.Diagnostics.Debug.WriteLine(userId);
            ApplicationUser user = db.Users.Find(userId);
            System.Diagnostics.Debug.WriteLine(user.Id);
            System.Diagnostics.Debug.WriteLine(UserManager.IsInRole(user.Id, "User"));
            if (UserManager.IsInRole(user.Id, "User") == true)
            {
                System.Diagnostics.Debug.WriteLine("User");

                UserManager.RemoveFromRole(userId, "User");
                UserManager.AddToRole(userId, "BlockedUser");
                TempData["mesaj"] = "User-ul a fost blocat cu succes!";
            }
            else if (UserManager.IsInRole(user.Id, "Seller") == true)
            {
                System.Diagnostics.Debug.WriteLine("Seller");

                var produse = from p in db.Produse
                              where p.idOwner == userId
                              select p;

                foreach (var p in produse)
                {
                    Produs produsDeAscuns = db.Produse.Find(p.idProdus);
                    produsDeAscuns.aprobat = false;
                }
                db.SaveChanges();

                UserManager.RemoveFromRole(userId, "Seller");
                UserManager.AddToRole(userId, "BlockedSeller");
                TempData["mesaj"] = "Seller-ul a fost blocat cu succes!";
            }
            else if (UserManager.IsInRole(user.Id, "BlockedUser") == true)
            {
                System.Diagnostics.Debug.WriteLine("BlockedUser");

                UserManager.RemoveFromRole(userId, "BlockedUser");
                UserManager.AddToRole(userId, "User");
                TempData["mesaj"] = "User-ul a fost deblocat cu succes!";
            }
            else if (UserManager.IsInRole(user.Id, "BlockedSeller") == true)
            {
                System.Diagnostics.Debug.WriteLine("BlockedSeller");

                UserManager.RemoveFromRole(userId, "BlockedSeller");
                UserManager.AddToRole(userId, "Seller");
                TempData["mesaj"] = "Seller-ul a fost deblocat cu succes!";
            }
            else if (UserManager.IsInRole(user.Id, "Admin") == true)
            {
                System.Diagnostics.Debug.WriteLine("Admin");

                TempData["mesaj"] = "Nu se poate bloca un cont administrativ";
            }

            return RedirectToAction("Index");
        }
    }
}