using Microsoft.AspNet.Identity;
using Proiect_ASP_final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP_final.Controllers
{
    [RequireHttps]
    public class AdresaController : Controller
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
                var adrese = from a in db.Adrese
                             select a;
                ViewBag.adrese = adrese;
                if (adrese.Any())
                    ViewBag.existaAdrese = true;
            }
            else
            {
                var adrese = from a in db.Adrese
                             where a.idUtilizator == userId
                             select a;
                ViewBag.adrese = adrese;
                if (adrese.Any())
                    ViewBag.existaAdrese = true;
            }


            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult Afisare(int id)
        {
            var isAdmin = false;
            if (User.IsInRole("Admin"))
                isAdmin = true;

            Adresa adresaCautata = db.Adrese.Find(id);

            if (adresaCautata.idUtilizator != User.Identity.GetUserId() && isAdmin == false)
            {
                TempData["mesaj"] = "Nu aveți permisiunea să vizualizați această adresă";
                return RedirectToAction("Index");
            }

            var comenzi = (from c in db.Comenzi
                           where c.idAdresa == id
                           select c).OrderBy(comanda => comanda.dataPlasare);

            ViewBag.comenzi = comenzi;
            if (comenzi.Any())
                ViewBag.areComenzi = true;

            return View(adresaCautata);
        }

        [Authorize(Roles = "Admin,User")]
        public ActionResult Editare(int id)
        {
            var isAdmin = false;
            if (User.IsInRole("Admin"))
                isAdmin = true;

            Adresa adresaDeEditat = db.Adrese.Find(id);

            if (adresaDeEditat.idUtilizator != User.Identity.GetUserId() && isAdmin == false)
            {
                TempData["mesaj"] = "Nu aveți permisiunea să vizualizați această adresă";
                return RedirectToAction("Index");
            }

            return View("FormEditare", adresaDeEditat);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Editare(int id, Adresa adresaActualizata)
        {
            Adresa adresaDeEditat = db.Adrese.Find(id);

            try
            {
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(adresaDeEditat))
                    {
                        adresaDeEditat = adresaActualizata;
                        db.SaveChanges();

                        return RedirectToAction("Afisare", new { id = id });
                    }
                    else
                    {
                        return View("FormEditare", adresaActualizata);
                    }
                }
                else
                {
                    return View("FormEditare", adresaActualizata);
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [Authorize(Roles = "User")]
        public ActionResult Adaugare()
        {
            Adresa adresa = new Adresa();
            return View("FormAdaugare", adresa);
        }

        [HttpPost]
        public ActionResult Adaugare(Adresa adresaDeAdaugat)
        {
            var userId = User.Identity.GetUserId();
            adresaDeAdaugat.idUtilizator = userId;

            try
            {
                if (ModelState.IsValid)
                {
                    Adresa adresaAdaugata = db.Adrese.Add(adresaDeAdaugat);
                    db.SaveChanges();

                    return RedirectToAction("Afisare", new { id = adresaAdaugata.idAdresa });
                }
                else
                {
                    return View("FormAdaugare", adresaDeAdaugat);
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Stergere(int id)
        {
            var isAdmin = false;
            if (User.IsInRole("Admin"))
                isAdmin = true;

            try
            {
                string userId = User.Identity.GetUserId();
                Adresa adresaDeSters = db.Adrese.Find(id);
                var comenzi = (from c in db.Comenzi
                              where c.idUtilizator == userId
                              select c).ToList<Comanda>();

                if (adresaDeSters.idUtilizator != User.Identity.GetUserId() && isAdmin == false || comenzi.Any())
                {
                    TempData["mesaj"] = "Nu aveți permisiunea să ștergeți acestă adresă";
                    return RedirectToAction("Index");
                }

                db.Adrese.Remove(adresaDeSters);
                db.SaveChanges();
                TempData["mesaj"] = "Adresa a fost stearsa cu succes!";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}