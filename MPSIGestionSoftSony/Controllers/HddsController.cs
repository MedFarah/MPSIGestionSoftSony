using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MPSIGestionSoftSony.Models;
using MPSIGestionSoftSony.Utilities;


namespace MPSIGestionSoftSony.Controllers
{
    [Authorize]
    [NoCache]
    public class HddsController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: Hdds
        [Authorize]
        public ActionResult Index()
        {
            //var hdd = db.hdd.Include(h => h.poste).Where(x =>x.Poste_Id==x.poste.Id && x.poste.Idligne == x.poste.ligne.Id && x.poste.ligne.IdAtelier == x.poste.ligne.atelier.Id ).ToList()
            // .Select(x =>
            //    new hdd()
            //    {
            //        NumSerie = x.NumSerie,
            //        Categorie = x.Categorie,
            //        Soft = x.Soft,
            //        Description = x.Description,
            //        Dt_achat = x.Dt_achat,
            //        Dt_sortie = x.Dt_sortie,
            //        Etat = x.Etat, 
            //        Poste_Id = x.Poste_Id,
            //        poste = new poste{Poste1 = x.poste.Poste1+"__"+x.poste.ligne.Ligne1+"__"+x.poste.ligne.atelier.Etage }
                    
            //    }).ToList();
            ViewBag.HDD = "HDD";
            return View();
        }
        //Get HDD par emplacement
        [Authorize]
        [NoCache]
        public ActionResult Emplacement(string Ligne)
        {

            var Hdd = db.hdd.SqlQuery("SELECT * FROM hdd, atelier, ligne, poste where  hdd.poste_Id = poste.Id and poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id and ligne.Ligne ='" + Ligne + "'").ToList();
            if (Ligne == null) { ViewBag.HDD = "ALL"; } else { ViewBag.HDD = Ligne.ToUpper(); }
            ViewBag.Active = Ligne;
            TempData["ligne"] = Ligne;
            return View("Index",Hdd);
        }

        //Get HDD par Etat HS
        [Authorize]
        [NoCache]
        public ActionResult EmplacementParEtat()
        {
            var Hdd = db.hdd.Where(x=>x.Etat=="HS").ToList();
            ViewBag.HDD ="Hors Stock";
            return View("Index", Hdd);
        }
        // GET: Hdds/Create
        [Authorize(Roles = "Admin,Technicien")]
        public ActionResult Create()
        {
            //Poste contient max 12 HDD
            ViewBag.Poste_Id = new SelectList(db.poste.Include(x => x.hdd).Include(x=>x.ligne).Where(x => x.hdd.Count() < 12).Where( x=> x.Idligne == x.ligne.Id && x.ligne.IdAtelier == x.ligne.atelier.Id &&( x.ligne.Ligne1 == "MCB" || x.ligne.Ligne1 == "ps4/ps3" || x.ligne.Ligne1 == "Stock")).ToList()
             .Select(x =>
                new poste()
                {
                    Id= x.Id,
                    Poste1 = x.Poste1+"__"+x.ligne.Ligne1+"__"+x.ligne.atelier.Etage
                }
              ), "Id", "Poste1");
            return View();
        }

        // POST: Hdds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NumSerie,Categorie,Soft,Poste_Id,Description,Etat,Dt_achat,Dt_sortie")] hdd hdd)
        {
            ViewBag.existdeja = null;
            if (ModelState.IsValid)
            {
                var numseriexist = db.hdd.Find(hdd.NumSerie);
                if (numseriexist == null)
                {
                    db.hdd.Add(hdd);
                    TempData["success"] = "success";
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.existdeja = "deja exist";
                    //var liste_postee = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
                    // ViewBag.poste_Id = new SelectList(liste_postee, "Id", "Poste1", equipement.poste_Id);
                    ViewBag.Poste_Id = new SelectList(db.poste.Include(x => x.hdd).Include(x => x.ligne).Where(x => x.hdd.Count() < 12).Where(x => x.Idligne == x.ligne.Id && x.ligne.IdAtelier == x.ligne.atelier.Id && (x.ligne.Ligne1 == "MCB" || x.ligne.Ligne1 == "ps4/ps3" || x.ligne.Ligne1=="Stock")).ToList()
                                 .Select(x =>
                                    new poste()
                                    {
                                        Id = x.Id,
                                        Poste1 = x.Poste1 + "__" + x.ligne.Ligne1 + "__" + x.ligne.atelier.Etage
                                    }
                                  ), "Id", "Poste1");
                    return View(hdd);
                }
                return RedirectToAction("Create", "Hdds");
            }

            ViewBag.Poste_Id = new SelectList(db.poste.Include(x => x.hdd).Include(x => x.ligne).Where(x => x.hdd.Count() < 12).Where(x => x.Idligne == x.ligne.Id && x.ligne.IdAtelier == x.ligne.atelier.Id && (x.ligne.Ligne1 == "MCB" || x.ligne.Ligne1 == "ps4/ps3" || x.ligne.Ligne1 == "Stock")).ToList()
                         .Select(x =>
                            new poste()
                            {
                                Id = x.Id,
                                Poste1 = x.Poste1 + "__" + x.ligne.Ligne1 + "__" + x.ligne.atelier.Etage
                            }
                          ), "Id", "Poste1");
            return View(hdd);
        }

        // GET: Hdds/Edit/
        [Authorize(Roles = "Admin,Technicien")]
        public ActionResult Edit(string id,string Ligne)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hdd hdd = db.hdd.Find(id);
            if (hdd == null)
            {
                return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Not Found');
                         window.location.href='/Hdds/Index'</script>");
            }
            ViewBag.Poste_Id = new SelectList(db.poste.Include(x => x.hdd).Include(x => x.ligne).Where(x => x.hdd.Count() < 12).Where(x => x.Idligne == x.ligne.Id && x.ligne.IdAtelier == x.ligne.atelier.Id && (x.ligne.Ligne1 == "MCB" || x.ligne.Ligne1 == "ps4/ps3" || x.ligne.Ligne1 == "Stock")).ToList()
             .Select(x =>
                new poste()
                {
                    Id = x.Id,
                    Poste1 = x.Poste1 + "__" + x.ligne.Ligne1 + "__" + x.ligne.atelier.Etage
                }
              ), "Id", "Poste1",hdd.Poste_Id);
            ViewBag.Categorie = hdd.Categorie;
            return View(hdd);
        }

        // POST: Hdds/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NumSerie,Categorie,Soft,Poste_Id,Description,Etat,Dt_achat,Dt_sortie")] hdd hdd)
        {
            TempData["name"] = null;
            
            if (ModelState.IsValid)
            {
                db.Entry(hdd).State = EntityState.Modified;
                db.SaveChanges();
                TempData["name"] = "HDD modifier";
                //return RedirectToAction("Index");
                return RedirectToAction("Emplacement", "Hdds", new
                {
                    Ligne = TempData["ligne"],
                });
            }
            ViewBag.Poste_Id = new SelectList(db.poste.Include(x => x.hdd).Include(x => x.ligne).Where(x => x.hdd.Count() < 12).Where(x => x.Idligne == x.ligne.Id && x.ligne.IdAtelier == x.ligne.atelier.Id && (x.ligne.Ligne1 == "MCB" || x.ligne.Ligne1 == "ps4/ps3" || x.ligne.Ligne1 == "Stock")).ToList()
                         .Select(x =>
                            new poste()
                            {
                                Id = x.Id,
                                Poste1 = x.Poste1 + "__" + x.ligne.Ligne1 + "__" + x.ligne.atelier.Etage
                            }
                          ), "Id", "Poste1", hdd.Poste_Id);
            return View(hdd);
        }

        //// GET: Hdds/Delete/id
        //[Authorize(Roles = "Admin")]
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    hdd hdd = db.hdd.Find(id);
        //    if (hdd == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(hdd);
        //}

        // POST: Hdds/Delete/id
        [ ActionName("Delete")]
        [HttpPost]
        [Authorize(Roles = "Admin,Technicien")]
        public ActionResult DeleteConfirmed(string id)
        {
            
            hdd hdd = db.hdd.Find(id);
            db.hdd.Remove(hdd);
            db.SaveChanges();
            if (TempData["url"] == null) { return RedirectToAction("Equipements"); } else { string url = TempData["url"].ToString(); return Redirect(url); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
