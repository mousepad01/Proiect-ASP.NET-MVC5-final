﻿using Proiect_ASP_final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP_final.Controllers
{   
    [RequireHttps]
    public class AdresaController : Controller
    {
        // IDEE PENTRU ADRESA
        // Gasisem un API cu toate tarile -> zonele -> orasele din ele pe care-l putem folosi (sa nu introduca utilizatorul aiurea)
        private Models.ApplicationDbContext db = new ApplicationDbContext();

        // GET: Adresa
        public ActionResult Index()
        {
            var adrese = from a in db.Adrese
                         select a;

            ViewBag.adrese = adrese;

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        /*
		Aici ar trebui sa avem un index separat cu id ca parametru sa dam doar 
        adresele unui utilizator (nu ne trebuie toate decat cand avem de-a face cu un admin)
		dar nu avem nevoie atata timp cat nu avem utilizatori
        public ActionResult Index(int i)
        {
            var adrese = from a in db.Adrese
                         where a.idUtilizator == i
                         select a;

            ViewBag.adrese = adrese;

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }
        */

        public ActionResult Afisare(int id)
        {
            Adresa adresaCautata = db.Adrese.Find(id);
            return View(adresaCautata);
        }

        public ActionResult Editare(int id)
        {
            Adresa adresaDeEditat = db.Adrese.Find(id);

            return View("FormEditare", adresaDeEditat);
        }

        [HttpPut]
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

        public ActionResult Adaugare()
        {
            Adresa adresa = new Adresa();
            return View("FormAdaugare", adresa);
        }

        [HttpPost]
        public ActionResult Adaugare(Adresa adresaDeAdaugat)
        {

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
        public ActionResult Stergere(int id)
        {
            try
            {
                Adresa adresaDeSters = db.Adrese.Find(id);
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