using Proiect_ASP_final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP_final.Controllers
{
    [RequireHttps]
    public class CategorieController : Controller
    {
        private Models.ApplicationDbContext db = new ApplicationDbContext();

        // GET: Lista categorii
        [AllowAnonymous]
        public ActionResult Index()
        {
            var categorii = from c in db.Categorii
                            select c;

            ViewBag.categorii = categorii;

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        // GET: Afisarea unei categorii (si produsele asociate)
        [AllowAnonymous]
        public ActionResult Afisare(int id)
        {
            Categorie categorieCautata = db.Categorii.Find(id);

            //produsele asociate categoriei cautate (prin intermediul tabelei CategoriiProduse)
            var produse = from p in db.Produse
                          from cp in db.CategoriiProduse
                          where cp.idCategorie == categorieCautata.idCategorie
                          where p.idProdus == cp.idProdus
                          where p.aprobat == true
                          select p;

            ViewBag.produse = produse;

            return View(categorieCautata);
        }

        //GET: Afisarea form ului pentru editarea atributelor unei categorii
        [Authorize(Roles = "Admin")]
        public ActionResult Editare(int id)
        {
            Categorie categorieDeEditat = db.Categorii.Find(id);

            return View("FormEditare", categorieDeEditat);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Editare(int id, Categorie categorieActualizata)
        {
            Categorie categorieDeEditat = db.Categorii.Find(id);

            try
            {
                Categorie categAcelasiNume = (from categ in db.Categorii
                                              where categ.titlu == categorieActualizata.titlu
                                              select categ).SingleOrDefault();

                bool numeUnic = categAcelasiNume == null;

                if (ModelState.IsValid && numeUnic)
                {
                    if (TryUpdateModel(categorieDeEditat))
                    {
                        categorieDeEditat = categorieActualizata;
                        db.SaveChanges();

                        return RedirectToAction("Afisare", new { id = id });
                    }
                    else
                    {
                        //ViewBag.categorie = categorieDeEditat;
                        categorieActualizata.idCategorie = categorieDeEditat.idCategorie;
                        return View("FormEditare", categorieActualizata);
                    }
                }
                else
                {   
                    if(!numeUnic)
                        TempData["eroareTitluUnic"] = "Există deja o categorie cu acest nume!";

                    categorieActualizata.idCategorie = categorieDeEditat.idCategorie;
                    return View("FormEditare", categorieActualizata);
                }

            }
            catch (Exception e)
            {
                ViewBag.exception = e;

                return View("ExceptieEditare");
            }
        }

        //GET: afisarea form ului de adaugare a unei noi categorii
        [Authorize(Roles = "Admin")]
        public ActionResult Adaugare()
        {
            Categorie categ = new Categorie();
            return View("FormAdaugare");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Adaugare(Categorie categorieDeAdaugat)
        {
            try
            {
                Categorie categAcelasiNume = (from categ in db.Categorii
                                             where categ.titlu == categorieDeAdaugat.titlu
                                             select categ).SingleOrDefault();

                bool numeUnic = categAcelasiNume == null;

                if (ModelState.IsValid && numeUnic)
                {
                    Categorie categorieAdaugata = db.Categorii.Add(categorieDeAdaugat);  // returneaza obiectul adaugat
                    db.SaveChanges();

                    return RedirectToAction("Afisare", new { id = categorieAdaugata.idCategorie });
                }
                else
                {
                    if (!numeUnic)
                        TempData["eroareTitluUnic"] = "Există deja o categorie cu acest nume!";

                    return View("FormAdaugare", categorieDeAdaugat);
                }
            }
            catch (Exception e)
            {
                ViewBag.exceptie = e;

                return View("ExceptieAdaugare");
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Stergere(int id)
        {
            try
            {
                Categorie categorieDeSters = db.Categorii.Find(id);
                db.Categorii.Remove(categorieDeSters);

                db.SaveChanges();

                TempData["mesaj"] = "Categoria a fost ștearsă cu succes!";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewBag.exceptie = e;

                return View("ExceptieStergere");
            }
        }
    }
}