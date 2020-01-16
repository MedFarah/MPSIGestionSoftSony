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
    [Authorize(Roles = "Admin,Technicien")]
    [NoCache]
    public class PannehddsController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: Pannehdds
        [Authorize]
        public ActionResult Index()
        {
            var pannehdd = db.pannehdd.Include(p => p.hdd).Include(p => p.panne);
            return View(pannehdd.ToList());
        }

        // GET: Pannehdds/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Hdd_NumSerie = new SelectList(db.hdd, "NumSerie", "NumSerie");
            ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne");
            return View();
        }

        // POST: Pannehdds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Hdd_NumSerie,Panne_Id,DatePH")] pannehdd pannehdd)
        {
            ViewBag.existdeja = null;
            if (ModelState.IsValid)
            {
                var numseriexist = db.pannehdd.Where(x => x.Hdd_NumSerie == pannehdd.Hdd_NumSerie && x.Panne_Id==pannehdd.Panne_Id).Count();
                if (numseriexist == 0)
                {
                    //Add to Historique
                    historiquepannehdd hph = new historiquepannehdd();
                    hph.DatePH = pannehdd.DatePH;
                    hph.Hdd_NumSerie = pannehdd.Hdd_NumSerie;
                    var panne = db.panne.Find(pannehdd.Panne_Id);
                    hph.Panne_Nom = panne.NomPanne;
                    db.historiquepannehdd.Add(hph);
                    db.SaveChanges();
                    var hdd = db.hdd.Find(pannehdd.Hdd_NumSerie);
                    if (ChecknbrPanne(hph))
                    {
                        //Set HDD en Panne
                        hdd.Etat = "En Panne";
                        db.pannehdd.Add(pannehdd);
                        TempData["created"] = "panne hdd ajouter";
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        //Set HDD destroyed
                        hdd.Etat = "HS";
                        hdd.Poste_Id = null;
                        db.SaveChanges();
                        TempData["HDDdestroyed"] = "this is the third time in a week";
                        TempData["HDDNumSerie"] = hdd.NumSerie;
                        return RedirectToAction("EmplacementParEtat", "Hdds");
                    }
                   
                }
                else
                {
                    ViewBag.existdeja = "deja exist";
                    ViewBag.Hdd_NumSerie = new SelectList(db.hdd, "NumSerie", "NumSerie", pannehdd.Hdd_NumSerie);
                    ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne", pannehdd.Panne_Id);
                    return View("Create");
                }
            }

            ViewBag.Hdd_NumSerie = new SelectList(db.hdd, "NumSerie", "NumSerie", pannehdd.Hdd_NumSerie);
            ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne", pannehdd.Panne_Id);
            return View(pannehdd);
        }

        // GET: Pannehdds/Edit/idHdd/idPanne
        [Authorize]
        public ActionResult Edit(string idH, int? idP)
        {
            if (idH == null || idP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pannehdd pannehdd = db.pannehdd.Where(panneE => panneE.Hdd_NumSerie == idH && panneE.Panne_Id == idP).FirstOrDefault();
            if (pannehdd == null)
            {
                return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Not Found');
                         window.location.href='/Pannehdds/Index'</script>");
            }
            ViewBag.Hdd_NumSerie = new SelectList(db.hdd, "NumSerie", "NumSerie", pannehdd.Hdd_NumSerie);
            ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne", pannehdd.Panne_Id);
            return View(pannehdd);
        }

        // POST: Pannehdds/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Hdd_NumSerie,Panne_Id,DatePH")] pannehdd pannehdd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pannehdd).State = EntityState.Modified;
                db.SaveChanges();
                TempData["updated"] = "updated";
                return RedirectToAction("Index");
            }
            ViewBag.Hdd_NumSerie = new SelectList(db.hdd, "NumSerie", "NumSerie", pannehdd.Hdd_NumSerie);
            ViewBag.Panne_Id = new SelectList(db.panne, "Id", "NomPanne", pannehdd.Panne_Id);
            return View(pannehdd);
        }

        
        // POST: PanneEquipements/Delete/5
        [ ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(string idH, int idP)
        {
            //Set HDD en Service
            var hdd = db.hdd.Find(idH);
            hdd.Etat = "En Service";
            pannehdd pannehdd = db.pannehdd.Where(panneE => panneE.Hdd_NumSerie == idH && panneE.Panne_Id == idP).FirstOrDefault();
            db.pannehdd.Remove(pannehdd);
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
        public Boolean ChecknbrPanne(historiquepannehdd h)
        {
           var list =  db.historiquepannehdd.Where(x => x.Hdd_NumSerie == h.Hdd_NumSerie && x.Panne_Nom == h.Panne_Nom).ToList();
            Boolean resultat = true;
            if (list.Count() < 3) { return resultat; }
            else
            {
                var lst = list.OrderByDescending(x => x.DatePH).Take(3);
                if ((lst.First().DatePH- lst.Last().DatePH).TotalDays < 7 )
                {
                    resultat = false;
                }
            }
            return resultat;
        }
    }
}
