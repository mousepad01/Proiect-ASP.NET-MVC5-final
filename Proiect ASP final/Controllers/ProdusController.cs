﻿using Microsoft.AspNet.Identity;
using Proiect_ASP_final.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP_final.Controllers
{   
    [RequireHttps]
    public class ProdusController : Controller
    {
        private Models.ApplicationDbContext db = new ApplicationDbContext();

        // GET: Lista produses
        [AllowAnonymous]
        public ActionResult Index()
        {
            var produse = from p in db.Produse
                          select p;

            // pentru a afisa intervalele default de filtre
            int pretMax = produse.Max(produs => produs.pret);
            int pretMin = produse.Min(produs => produs.pret);

            DateTime dataMax = produse.Max(produs => produs.dataAdaugare);
            DateTime dataMin = produse.Min(produs => produs.dataAdaugare);

            ViewBag.produse = produse;

            ViewBag.pretMax = pretMax;
            ViewBag.pretMin = pretMin;

            ViewBag.pretMinDefault = pretMin;
            ViewBag.pretMaxDefault = pretMax;

            ViewBag.dataMax = dataMax;
            ViewBag.dataMin = dataMin;

            ViewBag.dataMaxDefault = dataMax;
            ViewBag.dataMinDefault = dataMin;

            ViewBag.selectedSort = "none";

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        // POST PENTRU CĂ AȘA VREA FORMCOLLECTION, DAR PUTEA SĂ FIE GET: Lista produse sortate
        [HttpPost]
        [AllowAnonymous]
        public ActionResult IndexSorted(FormCollection result)
        {
            // FormCollection permite form-ului să fie transmis mai fancy și modificat ușor

            // Intervalele default din filtre 
            var produse = from p in db.Produse
                          select p;

            int pretMax = produse.Max(produs => produs.pret);
            int pretMin = produse.Min(produs => produs.pret);

            DateTime dataMax = produse.Max(produs => produs.dataAdaugare);
            DateTime dataMin = produse.Min(produs => produs.dataAdaugare);

            ViewBag.pretMax = pretMax;
            ViewBag.pretMin = pretMin;
            ViewBag.dataMax = dataMax;
            ViewBag.dataMin = dataMin;

            int pretMinAux = Convert.ToInt32(result["pretMin"]);
            int pretMaxAux = Convert.ToInt32(result["pretMax"]);
            DateTime dataMinAux = Convert.ToDateTime(result["dataMin"]);
            DateTime dataMaxAux = Convert.ToDateTime(result["dataMax"]);

            ViewBag.pretMinDefault = pretMinAux;
            ViewBag.pretMaxDefault = pretMaxAux;

            ViewBag.dataMaxDefault = dataMaxAux;
            ViewBag.dataMinDefault = dataMinAux;

            // la filtrarea dupa data, nu voi lua ora in calcul 
            // de aceea voi seta datetime ul maxim cu o zi inainte, pentru a lua toate adaugarile din acea zi
            dataMaxAux = dataMaxAux.AddDays(1);
           
            produse = from p in db.Produse
                      where p.pret >= pretMinAux && p.pret <= pretMaxAux
                      where p.dataAdaugare >= dataMinAux && p.dataAdaugare <= dataMaxAux 
                      select p;

            string sortCrit = result["sortCrit"];

            if (sortCrit == "tc")
                produse = produse.OrderBy(produs => produs.titlu);
            else if (sortCrit == "tdc")
                produse = produse.OrderByDescending(produs => produs.titlu);
            else if (sortCrit == "pc")
                produse = produse.OrderBy(produs => produs.pret);
            else if (sortCrit == "pdc")
                produse = produse.OrderByDescending(produs => produs.pret);
            else if (sortCrit == "dc")
                produse = produse.OrderBy(produs => produs.dataAdaugare);
            else if (sortCrit == "ddc")
                produse = produse.OrderByDescending(produs => produs.dataAdaugare);

            ViewBag.selectedSort = sortCrit;

            ViewBag.produse = produse;

            return View("Index");
        }

        // Returneaza o lista cu obiecte de tipul (valoare, text) 
        // unde valoarea este ID ul categoriei asociate (sub forma de string)
        // iar textul este TITLUL categoriei asociate (sub forma de string)
        [NonAction]
        private IEnumerable <SelectListItem> categoriiAsociate(Produs produs)
        {
            var categoriiAsociateQuery = from cp in db.CategoriiProduse
                                         from c in db.Categorii
                                         where cp.idProdus == produs.idProdus
                                         where cp.idCategorie == c.idCategorie
                                         select c;

            var categoriiAsociate = new List <SelectListItem>();
            
            foreach(var categ in categoriiAsociateQuery)
            {
                categoriiAsociate.Add(new SelectListItem
                {
                    Value = categ.idCategorie.ToString(),
                    Text = categ.titlu.ToString()
                });
            }

            return categoriiAsociate;
        }

        // Returneaza o lista cu obiecte de tipul (valoare, text) 
        // unde valoarea este ID ul categoriei neasociate (sub forma de string)
        // iar textul este TITLUL categoriei neasociate (sub forma de string)
        [NonAction]
        private IEnumerable <SelectListItem> categoriiNeasociate(Produs produs)
        {
            var categoriiAsociateQuery = from cp in db.CategoriiProduse
                                         from c in db.Categorii
                                         where cp.idProdus == produs.idProdus
                                         where cp.idCategorie == c.idCategorie
                                         select c;

            var categoriiNeasociateQuery = (from c in db.Categorii
                                            select c).Except(categoriiAsociateQuery);

            var categoriiNeasociate = new List <SelectListItem>();

            foreach(var categ in categoriiNeasociateQuery)
            {
                categoriiNeasociate.Add(new SelectListItem
                {
                    Value = categ.idCategorie.ToString(),
                    Text = categ.titlu.ToString()
                });
            }

            return categoriiNeasociate;
        }

        // overload pe metoda categoriiNeasociate, care primeste in plus lista de categorii asociate
        [NonAction]
        private IEnumerable <SelectListItem> categoriiNeasociate(Produs produs, IEnumerable <SelectListItem> categoriiAsociate)
        {
            var categoriiNeasociate = new List <SelectListItem>();
            
            var categoriiQuery = from c in db.Categorii
                                 select c;

            foreach(var categ in categoriiQuery)
            {
                var categPending = new SelectListItem{ Value = categ.idCategorie.ToString(), Text = categ.titlu.ToString()};

                if (!categoriiAsociate.Any(c => c.Value == categPending.Value))
                    categoriiNeasociate.Add(categPending);
            }

            return categoriiNeasociate;
        }

        // metoda care returneaza un array de categorii asociat unui array de ID uri primite ca parametru
        [NonAction]
        private Categorie[] categoriiDinId(int[] categoriiId)
        {   
            Categorie[] categoriiCorespunzatoare = new Categorie[categoriiId.Length];

            for (int i = 0; i < categoriiId.Length; i++)
            {
                int categId = categoriiId[i];

                var categ = from c in db.Categorii
                            where c.idCategorie == categId
                            select c;

                categoriiCorespunzatoare[i] = categ.SingleOrDefault();
            }

            return categoriiCorespunzatoare;
        }

        // overload pe metoda categoriiAsociate, care primeste in plus lista de ID uri ale categoriilor asociate
        // si returneaza obiectul de tip IEnumerable corespunzator array ului de ID uri trimis ca parametru
        [NonAction]
        private IEnumerable <SelectListItem> categoriiAsociate(Produs produs, int[] categoriiAsociateId)
        {
            Categorie[] categoriiAsociateArray;

            var categoriiAsociateRez = new List<SelectListItem>();

            try
            {
                categoriiAsociateArray = categoriiDinId(categoriiAsociateId);

                for (int i = 0; i < categoriiAsociateArray.Length; i++)
                {
                    categoriiAsociateRez.Add(new SelectListItem
                    {
                        Value = categoriiAsociateArray[i].idCategorie.ToString(),
                        Text = categoriiAsociateArray[i].titlu.ToString()
                    });
                }

                return categoriiAsociateRez;
            }
            catch (NullReferenceException)
            {
                return categoriiAsociateRez;
            }
            
           
        }

        // GET: Afisarea unui produs (si a categoriilor asociate)
        [AllowAnonymous]
        public ActionResult Afisare(int id)
        {
            Produs produsDeAfisat = db.Produse.Find(id);

            produsDeAfisat.CategoriiAsociate = categoriiAsociate(produsDeAfisat);

            // vreau sa imi afiseze rating urile proprii primele

            string currentUser = User.Identity.GetUserId();

            var ratinguriProprii = from pr in db.ProduseRatinguri
                                   where pr.idProdus == id
                                   where pr.idUtilizator == currentUser
                                   select pr;

            var ratinguri = from pr in db.ProduseRatinguri
                            where pr.idProdus == id
                            where pr.idUtilizator != currentUser
                            select pr;

            ratinguri = ratinguriProprii.Concat(ratinguri);

            ViewBag.ratinguri = ratinguri;

            // verificare drepturi pentru vizualizat butoane de stergere si editare

            // adminii pot sterge orice produs
            // in schimb, produsul nu poate fi editat decat de owner (nici macar de admin, decat daca ii apartine)
            if(produsDeAfisat.idOwner == User.Identity.GetUserId())
            {
                ViewBag.accesStergereProdus = true;
                ViewBag.accesEditareProdus = true;
            }
            else if (User.IsInRole("Admin"))
            {
                ViewBag.accesStergereProdus = true;
            }

            // daca valoarea este true, va afisa automat partial view ul de adaugare, 
            // este folosit si pentru afisarea erorii la adaugarea unui nou comentariu
            if (TempData["eroareRatingAdaugat"] != null)
            {
                
                ViewBag.EroareNouRating = true;

                ProdusRating ratingEronatAdaugat = TempData["ratingEronatAdaugat"] as ProdusRating;
                ViewBag.ratingEronatAdaugare = ratingEronatAdaugat;

                ViewBag.eroare = TempData["eroare"];
            }  
            else
                ViewBag.EroareNouRating = false;

            if (TempData["eroareRatingEditat"] != null)
            {
                ViewBag.EroareEditareRating = TempData["eroareRatingEditat"];

                ProdusRating ratingEronatEditat = TempData["ratingEronatEditat"] as ProdusRating;
                ViewBag.ratingEronatEditare = ratingEronatEditat;

                ViewBag.eroare = TempData["eroare"];
            }
            else
                ViewBag.EroareEditareRating = -1; // id de rating care nu exista

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View(produsDeAfisat);
        }

        // metoda care preia informatia din ambele form uri de editare (ale categoriilor, respectiv ale celorlalte informatii)
        [HttpPut]
        [Authorize(Roles = "Seller,Admin")]
        public ActionResult SwitchEditare(int id, Produs produsActualizat, int[] categoriiActualizateId)
        {   
            // deja am facut verificarea permisiunii de editare la afisarea formularului de editate
            // dar verific si aici pentru cazul cand ar face request direct cu tot cu query string ul corespunzator

            Produs produsDeEditat = db.Produse.Find(id);
            if(User.Identity.GetUserId() == produsDeEditat.idOwner)
            {
                // apelez pe rand metodele de modificare (separate) ale categoriilor, respectiv ale celorlalte informatii
                // verific, separat, daca aceste metode au modificat cu succes
                // iar daca cel putin una nu a reusit, reintorc in formularul de editare
                // cu mesajele de eroare necesare, dar cu campurile deja completate retinute

                bool ok_categ = StergeCategoriiAsociate(id, categoriiActualizateId);

                if (!ok_categ)
                    ViewBag.eroareCategoriiLipsa = "Trebuie să selectați cel puțin o categorie";

                bool ok_rest = Editare(id, produsActualizat);

                if (!ok_categ || !ok_rest)
                {
                    produsActualizat.idProdus = id;

                    produsActualizat.CategoriiAsociate = categoriiAsociate(produsActualizat, categoriiActualizateId);
                    produsActualizat.CategoriiNeasociate = categoriiNeasociate(produsActualizat, produsActualizat.CategoriiAsociate);

                    return View("FormEditare", produsActualizat);
                }
                else
                {
                    TempData["mesaj"] = "Produsul a fost editat cu succes!";

                    return RedirectToAction("Afisare", new { id = id });
                }
            }
            else
            {
                TempData["mesaj"] = "Nu aveți permisiunea să editați acest produs";

                return RedirectToAction("Index");
            }
            
        }

        // Adauga in tabela CategoriiProduse inregistrari cu id-ul dat si categoriile asociate preluate
        // (se are in vedere ca metoda care o apeleaza sterge vechile categorii de dinainte)
        //
        // Am ales sa implementez asa deoarece ar fi existat situatii cand trebuia sa sterg vechile categorii
        // Si in acelasi timp sa adaug altele noi, deci in cel mai rau caz oricum aveam de apelat doua metode
        // Pentru a simplifica implementarea, le apelez de fiecare data, 
        // Renuntand la ideea de a imparti situatia in mai multe cazuri particulare
        [NonAction]
        private bool AdaugaCategoriiAsociate(int id, Categorie[] categoriiDeAdaugat)
        {
            try
            {
                for (int i = 0; i < categoriiDeAdaugat.Length; i++)
                {
                    var categIdAux = categoriiDeAdaugat[i].idCategorie;

                    var categProd = from cp in db.CategoriiProduse
                                    where cp.idCategorie == categIdAux
                                    where cp.idProdus == id
                                    select cp;

                    var auxCategProd = categProd.SingleOrDefault();

                    db.CategoriiProduse.Add(new CategorieProdus { idProdus = id, idCategorie = categoriiDeAdaugat[i].idCategorie });
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Prima metoda (a doua fiind AdaugaCategoriiAsociate) 
        // folosita in vederea modificarii categoriilor din care un produs face parte
        // STERGE vechile categorii asociate
        [NonAction]
        private bool StergeCategoriiAsociate(int id, int[] categoriiActualizateId)
        {
            try
            {
                Categorie[] categoriiActualizate = categoriiDinId(categoriiActualizateId);

                var asocieri = from cp in db.CategoriiProduse
                               where cp.idProdus == id
                               select cp;

                foreach (var asoc in asocieri)
                    db.CategoriiProduse.Remove(asoc);

                db.SaveChanges();

                return AdaugaCategoriiAsociate(id, categoriiActualizate);
            }
            catch (Exception)
            {
                return false;
            }
        }

        //GET: Afisarea form ului pentru editarea unui produs 
        [Authorize(Roles = "Seller,Admin")]
        public ActionResult Editare(int id)
        {
            Produs produsDeEditat = db.Produse.Find(id);

            if(produsDeEditat.idOwner == User.Identity.GetUserId())
            {
                produsDeEditat.CategoriiNeasociate = categoriiNeasociate(produsDeEditat);
                produsDeEditat.CategoriiAsociate = categoriiAsociate(produsDeEditat);

                return View("FormEditare", produsDeEditat);
            }
            else
            {
                TempData["mesaj"] = "Nu aveți permisiunea să editați acest produs";

                return RedirectToAction("Index");
            }
            
        }

        // Editarea informatiilor unui produs FARA CATEGORIILE ASOCIATE
        [NonAction]
        public bool Editare(int id, Produs produsActualizat)
        {
            Produs produsDeEditat = db.Produse.Find(id);

            try
            {
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(produsDeEditat))
                    {
                        produsDeEditat = produsActualizat;
                        db.SaveChanges();

                        return true;
                    }
                    else
                    {
                        ViewBag.produs = produsDeEditat;
                        return false;
                    }
                }
                else
                    return false;
            }
            catch(Exception)
            {
                return false;
            }
        }

        //GET: afisarea form ului pentru adaugarea unui produs nou
        [Authorize(Roles = "Seller,Admin")]
        public ActionResult Adaugare()
        {
            Produs produs = new Produs();
            //produs.idOwner = User.Identity.GetUserId();

            // produsul, evident, nu are categorii asociate, dar apelez metodele pentru evitarea erorilor
            produs.CategoriiAsociate = categoriiAsociate(produs);
            produs.CategoriiNeasociate = categoriiNeasociate(produs);

            return View("FormAdaugare", produs);
        }

        [HttpPost]
        [Authorize(Roles = "Seller,Admin")]
        public ActionResult Adaugare(Produs produsDeAdaugat, int[] categoriiDeAdaugatId)
        {
            try
            {
                produsDeAdaugat.CategoriiAsociate = categoriiAsociate(produsDeAdaugat, categoriiDeAdaugatId);

                if (ModelState.IsValid && produsDeAdaugat.CategoriiAsociate.Count() > 0)
                {
                    Categorie[] categoriiDeAdaugat = categoriiDinId(categoriiDeAdaugatId);

                    produsDeAdaugat.dataAdaugare = DateTime.Now;

                    produsDeAdaugat.idOwner = User.Identity.GetUserId();
                    produsDeAdaugat.numeOwner = User.Identity.GetUserName();

                    db.Produse.Add(produsDeAdaugat);

                    for (int i = 0; i < categoriiDeAdaugatId.Length; i++)
                        db.CategoriiProduse.Add(new CategorieProdus { idProdus = produsDeAdaugat.idProdus, idCategorie = categoriiDeAdaugat[i].idCategorie });

                    db.SaveChanges();

                    return RedirectToAction("Afisare", new { id = produsDeAdaugat.idProdus });
                }
                else
                {
                    produsDeAdaugat.CategoriiNeasociate = categoriiNeasociate(produsDeAdaugat, produsDeAdaugat.CategoriiAsociate);

                    if(produsDeAdaugat.CategoriiAsociate.Count() == 0)
                        ViewBag.eroareCategoriiLipsa = "Produsul trebuie să facă parte din cel puțin o categorie!";

                    return View("FormAdaugare", produsDeAdaugat);
                }
                
            }
            catch (Exception e)
            {
                ViewBag.exceptie = e;

                return View("ExceptieAdaugare");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Seller,Admin")]
        public ActionResult Stergere(int id)
        {
            try
            {
                Produs produsDeSters = db.Produse.Find(id);

                if (produsDeSters.idOwner == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    db.Produse.Remove(produsDeSters);

                    db.SaveChanges();

                    TempData["mesaj"] = "Produsul a fost eliminat cu scces!";
                }
                else
                    TempData["mesaj"] = "Nu aveți dreptul să ștergeți acest produs!";
                

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ViewBag.exceptie = e;

                return View("ExceptieStergere");
            }
        }
    }
}