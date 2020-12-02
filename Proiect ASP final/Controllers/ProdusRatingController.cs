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
        // Ideea lui stefan pentru rating (poti sterge asta dupa ce ai citit): sa avem un singur create per user-produs (un utilizator poate adauga un singur rating pentru fiecare produs)
        // Asa ca potem face mai simplu ca form-ul primit de utilizatorul U sa fie fie de tip create (in caz ca utilizatorul n-a lasat rating)
        // Sau de edit in cazul in care utilizatorul U a lasat rating, lasandu-i astfel posibilitatea sa-l modifice
        // Desi era ft fancy ce ai facut cu jQuery, nu i-am facut front-end pentru ca m-a pierdut putin

        // Also planuiesc sa fac rating-ul cu 5 stelute din front-end (vezi cum are eMag ca sa-ti faci o idee)
        private Models.ApplicationDbContext db = new ApplicationDbContext();

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

            int produsAsociat = ratingDeSters.idProdus;

            if (User.Identity.GetUserId() == ratingDeSters.idUtilizator || User.IsInRole("Admin"))
            {
                try
                {
                    db.ProduseRatinguri.Remove(ratingDeSters);

                    db.SaveChanges();

                    TempData["mesaj"] = "Rating-ul a fost șters cu succes!";

                    return Redirect("/Produs/Afisare/" + produsAsociat);
                }
                catch (Exception)
                {
                    TempData["mesaj"] = "Eroare la ștergerea unui rating";
                    return Redirect("/Produs/Afisare/" + produsAsociat);
                }
            }
            else
            {
                TempData["mesaj"] = "Nu aveți dreptul sa ștergeți acest comentariu!";
                return Redirect("/Produs/Afisare/" + produsAsociat);
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
                            ratingDeEditat.rating = ratingActualizat.rating;
                            ratingDeEditat.descriere = ratingActualizat.descriere;
                            ratingDeEditat.dataReview = DateTime.Now;

                            db.SaveChanges();

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