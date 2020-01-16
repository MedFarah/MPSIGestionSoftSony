using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MPSIGestionSoftSony.Models;
using MPSIGestionSoftSony.Utilities;

namespace MPSIGestionSoftSony.Controllers
{
    [Authorize(Roles = "Admin,Technicien")]
    [NoCache]
    public class PanneEquipementsController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: PanneEquipements
        [Authorize]
        public ActionResult Index()
        {
            var panneequipement = db.panneequipement.Include(p => p.equipement).Include(p => p.panne);
            return View(panneequipement.ToList());
        }

        // GET: PanneEquipements/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Equipement_NumSerie = new SelectList(db.equipement, "NumSerie", "NumSerie");
            ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne");
            return View();
        }

        // POST: PanneEquipements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Panne_Id,Equipement_NumSerie,DatePE")] panneequipement panneequipement)
        {
            ViewBag.existdeja = null;

            if (ModelState.IsValid)
            {
                var numseriexist = db.panneequipement.Where(x => x.Equipement_NumSerie == panneequipement.Equipement_NumSerie && x.Panne_Id == panneequipement.Panne_Id).Count();
                if (numseriexist == 0)
                {
                    //Add to historique
                    historiquepanneequipement Hpe = new historiquepanneequipement();
                    Hpe.Nom_machine = db.equipement.Find(panneequipement.Equipement_NumSerie).NumSerie;
                    Hpe.Nom_panne = db.panne.Find(panneequipement.Panne_Id).NomPanne;
                    Hpe.DatePE = panneequipement.DatePE;
                    db.historiquepanneequipement.Add(Hpe);
                    db.SaveChanges();
                    var Equipement = db.equipement.Find(panneequipement.Equipement_NumSerie);
                    if (ChecknbrPanne(Hpe))
                    {
                        Equipement.Etat = "En panne";
                        db.panneequipement.Add(panneequipement);
                        db.SaveChanges();
                        TempData["created"] = "created";
                    } else
                    {
                        Equipement.Etat = "HS";
                        //Emplacement null
                        Equipement.poste_Id = null;
                        string categorie = Equipement.Categorie;
                        db.SaveChanges();
                        TempData["Equipementdestroyed"] = "this is the third time in a week";
                        TempData["EquipementNumSerie"] = Equipement.NumSerie;
                        return RedirectToAction("AffichageParEtat", "Equipements", new
                        {
                            Etat = "HS",
                            categorie
                            
                        });
                    }
                       
                }
                else
                {
                    ViewBag.existdeja = "deja exist";
                    ViewBag.Equipement_NumSerie = new SelectList(db.equipement, "NumSerie", "NumSerie");
                    ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne");
                    return View("Create");

                }

                return RedirectToAction("Index");
            }

            ViewBag.Equipement_NumSerie = new SelectList(db.equipement, "NumSerie", "NumSerie", panneequipement.Equipement_NumSerie);
            ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne", panneequipement.Panne_Id);
            return View(panneequipement);
        }

        // GET: PanneEquipements/Edit/5/5
        [Authorize]
        public ActionResult Edit(string idE , int? idP)
        {
            if (idE == null ||idP == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            panneequipement panneequipement = db.panneequipement.Where(panneE => panneE.Equipement_NumSerie == idE && panneE.Panne_Id==idP).FirstOrDefault();
            if (panneequipement == null)
            {
                return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Not Found');
                         window.location.href='/PanneEquipements/Index'</script>");
            }
            ViewBag.Equipement_NumSerie = new SelectList(db.equipement, "NumSerie", "NumSerie", panneequipement.Equipement_NumSerie);
            ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne", panneequipement.Panne_Id);
            return View(panneequipement);
        }

        // POST: PanneEquipements/Edit/5/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Panne_Id,Equipement_NumSerie,DatePE")] panneequipement panneequipement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(panneequipement).State = EntityState.Modified;
                db.SaveChanges();
                TempData["updated"] = "updated";
                return RedirectToAction("Index");
            }
            ViewBag.Equipement_NumSerie = new SelectList(db.equipement, "NumSerie", "NumSerie", panneequipement.Equipement_NumSerie);
            ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne", panneequipement.Panne_Id);
            return View(panneequipement);
        }

        
        // POST: PanneEquipements/Delete/5/5
        [ ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(string idE, int idP)
        {
            //Set equipement en service
            var Equipement = db.equipement.Find(idE);
            Equipement.Etat = "Service";
            panneequipement panneequipement = db.panneequipement.Where(panneE => panneE.Equipement_NumSerie == idE && panneE.Panne_Id == idP).FirstOrDefault();
            db.panneequipement.Remove(panneequipement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //verifier nombre panne dans une semaine
        public Boolean ChecknbrPanne(historiquepanneequipement h)
        {
            var list = db.historiquepanneequipement.Where(x => x.Nom_machine == h.Nom_machine && x.Nom_panne == h.Nom_panne).ToList();
            Boolean resultat = true;
            if (list.Count() < 3) { return resultat; }
            else
            {
                var lst = list.OrderByDescending(x => x.DatePE).Take(3);
                if ((lst.First().DatePE - lst.Last().DatePE).TotalDays < 7)
                {
                    resultat = false;
                }
            }
            return resultat;
        }
    
}
}
