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
    public class ProdusRatingController : Controller
    {
        private Models.ApplicationDbContext db = new ApplicationDbContext();

        // calcularea ratingului mediu al unui produs
        [NonAction]
        public int getRatingMediu(Produs produs)
        {
            if (produs.nrRatinguri == 0)
                return 0;
            else
            {
                if (produs.ratingInsumat % produs.nrRatinguri >= produs.nrRatinguri / 2)
                    return produs.ratingInsumat / produs.nrRatinguri + 1;
                else
                    return produs.ratingInsumat / produs.nrRatinguri;
            }
        }

        // metoda pentru a updata numarul de review uri si suma review urilor ale produsului asociat 
        [NonAction]
        public void UpdateRatingStat(int produsId, int rating, string flag)
        {
            Produs produsAsociat = db.Produse.Find(produsId);

            if (flag == "NOU")
                produsAsociat.nrRatinguri += 1;

            if (flag == "STERGE")
                produsAsociat.nrRatinguri -= 1;

            if (flag == "STERGE")
                produsAsociat.ratingInsumat -= rating;
            else
                produsAsociat.ratingInsumat += rating;

            produsAsociat.ratingMediu = getRatingMediu(produsAsociat);

            if (TryUpdateModel(produsAsociat))
            {
                db.SaveChanges();
            }

        }

        // GET : apelat de un script in jQuery, returneaza view ul partial pentru adaugarea unui comentariu nou
        // in aceasta metoda, parametrul ID este AL PRODUSULUI ASOCIAT !!!!!
        [Authorize(Roles = "Admin,Seller,User")]
        public ActionResult AdaugaRating(int id)
        {
            ProdusRating ratingDeAdaugat = new ProdusRating();
            ratingDeAdaugat.idProdus = id;

            return PartialView("InputRating", ratingDeAdaugat);
        }

        // in aceasta metoda, parametrul ID este AL PRODUSULUI ASOCIAT !!!!!
        [HttpPost]
        [Authorize(Roles = "Admin,Seller,User")]
        public ActionResult AdaugaRating(int id, ProdusRating ratingDeAdaugat)
        {
            ratingDeAdaugat.idProdus = id;
            ratingDeAdaugat.dataReview = DateTime.Now;

            ratingDeAdaugat.idUtilizator = User.Identity.GetUserId();
            ratingDeAdaugat.numeUtilizator = User.Identity.GetUserName();

            try
            {
                if (ModelState.IsValid)
                {
                    db.ProduseRatinguri.Add(ratingDeAdaugat);
                    db.SaveChanges();

                    UpdateRatingStat(id, ratingDeAdaugat.rating, "NOU");

                    return Redirect("/Produs/Afisare/" + ratingDeAdaugat.idProdus);
                }
                else
                {
                    TempData["eroareRatingAdaugat"] = true;
                    TempData["ratingEronatAdaugat"] = ratingDeAdaugat;
                    TempData["eroare"] = "Rating-ul trebuie să fe între 1 și 5 stele, si descrierea nu mai lungă de 1024 de caractere!";
                    
                    return Redirect("/Produs/Afisare/" + ratingDeAdaugat.idProdus);
                }
            }
            catch(Exception)
            {
                TempData["mesaj"] = "Eroare la adaugarea unui comentariu nou";
                return Redirect("/Produs/Afisare/" + ratingDeAdaugat.idProdus);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin,Seller,User")]
        public ActionResult StergeRating(int id)
        {
            ProdusRating ratingDeSters = db.ProduseRatinguri.Find(id);

            int idProdusAsociat = ratingDeSters.idProdus;

            if (User.Identity.GetUserId() == ratingDeSters.idUtilizator || User.IsInRole("Admin"))
            {
                try
                {
                    db.ProduseRatinguri.Remove(ratingDeSters);

                    db.SaveChanges();

                    UpdateRatingStat(idProdusAsociat, ratingDeSters.rating, "STERGE");

                    TempData["mesaj"] = "Rating-ul a fost șters cu succes!";

                    return Redirect("/Produs/Afisare/" + idProdusAsociat);
                }
                catch (Exception)
                {
                    TempData["mesaj"] = "Eroare la ștergerea unui rating";
                    return Redirect("/Produs/Afisare/" + idProdusAsociat);
                }
            }
            else
            {
                TempData["mesaj"] = "Nu aveți dreptul sa ștergeți acest comentariu!";
                return Redirect("/Produs/Afisare/" + idProdusAsociat);
            }

        }

        // GET : apelat de un script in jQuery, afiseaza form ul de editare al unui review
        [Authorize(Roles = "Admin,Seller,User")]
        public ActionResult EditeazaRating(int id)
        {
            ProdusRating ratingDeEditat = db.ProduseRatinguri.Find(id);

            if (User.Identity.GetUserId() == ratingDeEditat.idUtilizator)
            {
                return PartialView("EditareRating", ratingDeEditat);
            }
            else
            {
                TempData["mesaj"] = "Nu aveți permisiunea să editați acest rating!";

                return Redirect("Produs/Afisare/" + ratingDeEditat.idProdus);
            }

            
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Seller,User")]
        public ActionResult EditeazaRating(int id, ProdusRating ratingActualizat)
        {
            ProdusRating ratingDeEditat = db.ProduseRatinguri.Find(id);

            if(User.Identity.GetUserId() == ratingDeEditat.idUtilizator)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (TryUpdateModel(ratingDeEditat))
                        {
                            int ratingDif = ratingActualizat.rating - ratingDeEditat.rating;

                            ratingDeEditat.rating = ratingActualizat.rating;
                            ratingDeEditat.descriereRating = ratingActualizat.descriereRating;
                            ratingDeEditat.dataReview = DateTime.Now;

                            db.SaveChanges();

                            UpdateRatingStat(ratingDeEditat.idProdus, ratingDif, "EDITAT");

                            return Redirect("/Produs/Afisare/" + ratingDeEditat.idProdus);
                        }
                        else
                        {
                            TempData["mesaj"] = "Nu s-a putut edita review-ul";
                            return Redirect("/Produs/Afisare/" + ratingDeEditat.idProdus);
                        }
                    }
                    else
                    {
                        TempData["eroareRatingEditat"] = id;
                        ratingActualizat.prodRating = id;
                        TempData["ratingEronatEditat"] = ratingActualizat;
                        TempData["eroare"] = "Rating-ul trebuie să fe între 1 și 5 stele, si descrierea nu mai lungă de 1024 de caractere!";

                        return Redirect("/Produs/Afisare/" + ratingDeEditat.idProdus);
                    }
                }
                catch (Exception)
                {
                    TempData["mesaj"] = "Eroare la editarea review-ului!";
                    return Redirect("/Produs/Afisare/" + ratingDeEditat.idProdus);
                }
            }
            else
            {
                TempData["mesaj"] = "Nu aveți dreptul să editați acest review!";

                return Redirect("/Produs/Afisare/" + ratingDeEditat.idProdus);
            }
            
        }
    }
}