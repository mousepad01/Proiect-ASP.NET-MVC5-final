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
    public class ComandaController : Controller
    {
        private Models.ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "User,Admin")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            ViewBag.isAdmin = false;
            if (User.IsInRole("Admin"))
                ViewBag.isAdmin = true;

            if (ViewBag.isAdmin)
            {
                var comenzi = from a in db.Comenzi
                              select a;
                ViewBag.comenzi = comenzi;

                if (comenzi.Any())
                    ViewBag.areComenzi = true;
            }
            else
            {
                var comenzi = from a in db.Comenzi
                                  where a.idUtilizator == userId
                                  select a;

                ViewBag.comenzi = comenzi;

                if (comenzi.Any())
                    ViewBag.areComenzi = true;
            }
                

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Confirma(int id)
        {
            try
            {
                Comanda comandaDeConfirmat = db.Comenzi.Find(id);
                var isAdmin = false;
                if (User.IsInRole("Admin"))
                    isAdmin = true;

                if (isAdmin && comandaDeConfirmat.dataPlasare == comandaDeConfirmat.dataFinalizare)
                {
                    comandaDeConfirmat.dataFinalizare = DateTime.Now;
                    db.SaveChanges();
                    TempData["mesaj"] = "Comanda a fost confirmată cu succes!";
                    return RedirectToAction("Index");

                }

                TempData["mesaj"] = "Nu aveți permisiunea să confirmați această comandă!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin,User")]
        public ActionResult Afisare(int id)
        {
            Comanda comandaCautata = db.Comenzi.Find(id);
            Adresa adresa = db.Adrese.Find(comandaCautata.idAdresa);
            var produseComandate = from pc in db.ProduseComandate
                                   where pc.idComanda == id
                                   select pc;
            ViewBag.produseComandate = produseComandate;
            ViewBag.adresa = adresa;

            if (User.IsInRole("Admin"))
                ViewBag.isAdmin = true;

            if (comandaCautata.dataFinalizare == comandaCautata.dataPlasare)
                ViewBag.nefinalizata = true;

            return View(comandaCautata);
        }

        [Authorize(Roles = "User")]
        public ActionResult Adaugare()
        {
            String userId = User.Identity.GetUserId();
            Comanda comanda = new Comanda();

            comanda.idUtilizator = userId;
            comanda.Adrese = GetAllAdresses();

            return View("FormAdaugare", comanda);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult Adaugare(Comanda comandaDeAdaugat)
        {
            String userId = User.Identity.GetUserId();
            var cartItems = (from cp in db.CartItems
                            where cp.idUtilizator == userId
                            select cp).ToList<CartItem>();

            comandaDeAdaugat.dataPlasare = DateTime.Now;
            comandaDeAdaugat.dataFinalizare = DateTime.Now;
            comandaDeAdaugat.idUtilizator = userId;
            comandaDeAdaugat.sumaDePlata = 0;

            System.Diagnostics.Debug.WriteLine(comandaDeAdaugat.idAdresa);
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (CartItem cartItem in cartItems)
                    {
                        Produs produs = db.Produse.Find(cartItem.idProdus);
                        if (produs.cantitate != 0)
                        {
                            // Adaug un prouds nou comandat
                            ProdusComandat produsComandat = new ProdusComandat();
                            produsComandat.idComanda = comandaDeAdaugat.idComanda;
                            produsComandat.idProdus = produs.idProdus;
                            produsComandat.denumireProdus = produs.titlu;
                            // Scad cantitatea si adaug la suma pretul produsului * cantitate
                            int cantitateScazuta = cartItem.cantitate < produs.cantitate ? cartItem.cantitate : produs.cantitate;
                            produsComandat.cantitate = cantitateScazuta;
                            produs.cantitate -= cantitateScazuta;
                            comandaDeAdaugat.sumaDePlata += cantitateScazuta * produs.pret;
                            db.ProduseComandate.Add(produsComandat);

                            // Aici verific ca alte comenzi sa nu depaseasca pretul produselor (daca un produs nu mai e in stoc, se sterge automat din cosul utilizatorului)
                            var alteComenzi = (from ac in db.CartItems
                                              where ac.idProdus == produs.idProdus
                                              select ac).ToList<CartItem>();

                            if (produs.cantitate != 0)
                            {
                                foreach (CartItem altaComanda in alteComenzi)
                                {
                                    altaComanda.cantitate = altaComanda.cantitate < produs.cantitate ? altaComanda.cantitate : produs.cantitate;
                                    altaComanda.cantitateMaxima -= cantitateScazuta;
                                }
                            }
                            else
                            {
                                foreach (CartItem altaComanda in alteComenzi)
                                    db.CartItems.Remove(altaComanda);
                            }
                            
                            // Sterg produsul din cosul utilizatorului
                            CartItem cartItemDeSters = db.CartItems.Find(cartItem.id);
                            db.CartItems.Remove(cartItemDeSters);
                        }
                    }

                    db.Comenzi.Add(comandaDeAdaugat);
                    db.SaveChanges();
                       
                    return RedirectToAction("Afisare", new { id = comandaDeAdaugat.idComanda });
                }
                else
                {
                    comandaDeAdaugat.Adrese = GetAllAdresses();
                    return View("FormAdaugare", comandaDeAdaugat);
                }
            }
            // Catch-ul asta e aici doar in cazul in care mai apar erori cu comenzile (am avut destule)
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Stergere(int id)
        {
            try
            {
                Comanda comandaDeSters = db.Comenzi.Find(id);
                var isAdmin = false;
                if (User.IsInRole("Admin"))
                    isAdmin = true;

                if ((comandaDeSters.idUtilizator != User.Identity.GetUserId() && comandaDeSters.dataPlasare == comandaDeSters.dataFinalizare) || isAdmin == true)
                {
                    db.Comenzi.Remove(comandaDeSters);
                    db.SaveChanges();
                    TempData["mesaj"] = "Comanda a fost stearsa cu succes!";
                    return RedirectToAction("Index");
                }

                TempData["mesaj"] = "Nu este posibil să ștergeți această comandă!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllAdresses()
        {
            var selectList = new List<SelectListItem>();
            var adrese = from a in db.Adrese
                             select a;

            foreach (var adresa in adrese)
            {
                selectList.Add(new SelectListItem
                {
                    Value = adresa.idAdresa.ToString(),
                    Text = adresa.tara.ToString() + ", " + adresa.oras.ToString() + ", Str. " + adresa.strada.ToString() + ", Nr. " + adresa.nr.ToString()
                });
            }

            return selectList;
        }
    }
}