using Microsoft.AspNet.Identity;
using Proiect_ASP_final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP_final.Controllers
{
    [RequireHttps]
    public class CartItemController : Controller
    {
        private Models.ApplicationDbContext db = new ApplicationDbContext();

        // GET: CartItem
        [Authorize(Roles = "User")]
        public ActionResult Index()
        {
            String userId = User.Identity.GetUserId();
            var cartItems = (from cp in db.CartItems
                             where cp.idUtilizator == userId
                             select cp).ToList<CartItem>();

            if (cartItems.Count == 0)
                ViewBag.areProduse = false;
            else
                ViewBag.areProduse = true;

            ViewBag.cartItems = cartItems;

            int total = 0;
            foreach (var cartItem in cartItems)
            {
                var produs = (from p in db.Produse
                              where p.idProdus == cartItem.idProdus
                              select p).ToList<Produs>();
                total += produs[0].pret * cartItem.cantitate; // Se va selecta doar un produs și va fi o listă [x], deci e ok cu [0]
            }
            ViewBag.total = total;


            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult Modify(FormCollection result)
        {
            CartItem cartItem = db.CartItems.Find(Convert.ToInt32(result["Identifier"]));
            Produs produs = db.Produse.Find(cartItem.idProdus);

            if (produs.cantitate < Convert.ToInt32(result["Cantitate"]))
                TempData["mesaj"] = "Cantitatea cerută nu este disponibilă";
            else
            {
                TempData["mesaj"] = "Cantitatea a fost actualizată cu succes";
                cartItem.cantitate = Convert.ToInt32(result["Cantitate"]);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User")]
        public ActionResult Create(int idProdus)
        {
            String userId = User.Identity.GetUserId();
            Produs produs = db.Produse.Find(idProdus);

            var cartItems = (from cp in db.CartItems
                             where cp.idUtilizator == userId
                             where cp.idProdus == idProdus
                             select cp).ToList<CartItem>();

            if (cartItems.Any())
            {
                TempData["mesaj"] = "Produsul se află deja în coș.";
            }
            else if (produs.cantitate == 0)
            {
                TempData["mesaj"] = "Produsul nu este disponibil";
            }
            else
            {
                CartItem cartItem = new CartItem();

                cartItem.idProdus = idProdus;
                cartItem.idUtilizator = User.Identity.GetUserId();
                cartItem.cantitate = 1;
                cartItem.cantitateMaxima = produs.cantitate;
                cartItem.denumireProdus = produs.titlu;
                db.CartItems.Add(cartItem);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpDelete]
        [Authorize(Roles = "User")]
        public ActionResult Stergere(int id)
        {
            try
            {
                String userId = User.Identity.GetUserId();
                CartItem cartItemDeSters = db.CartItems.Find(id);

                if (cartItemDeSters.idUtilizator != userId)
                {
                    TempData["mesaj"] = "Produsul nu poate fi scos din coș!";
                    return RedirectToAction("Index");
                }

                db.CartItems.Remove(cartItemDeSters);
                db.SaveChanges();

                TempData["mesaj"] = "Produsul a fost scos din coș cu succes!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}