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
    public class PostesController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: Postes
        [Authorize]
        public ActionResult Index()
        {
            var poste = db.poste.Include(p => p.ligne);
            return View(poste.ToList());
        }



        // GET: Postes/Create
        [Authorize]
        public ActionResult Create()
        {
            //liste deroulante
            var listligne = db.ligne.SqlQuery("SELECT CONCAT(ligne.Ligne,'_', atelier.Etage) AS Ligne1,ligne.Id,ligne.IdAtelier FROM ligne, atelier Where ligne.IdAtelier = atelier.Id").ToList();
            ViewBag.Idligne = new SelectList(listligne, "Id", "Ligne1");
            return View();
        }

        // POST: Postes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Poste1,Idligne")] poste poste)
        {
            ViewBag.created = null;
            if (ModelState.IsValid)
            {
                db.poste.Add(poste);
                db.SaveChanges();
                ViewBag.created = "created";
            }
            else
            {

                var listligne = db.ligne.SqlQuery("SELECT CONCAT(ligne.Ligne,'_', atelier.Etage) AS Ligne1,ligne.Id,ligne.IdAtelier FROM ligne, atelier Where ligne.IdAtelier = atelier.Id").ToList();
                ViewBag.Idligne = new SelectList(listligne, "Id", "Ligne1");
                return View(poste);
            }
            return RedirectToAction("Index");
        }

        // GET: Postes/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            poste poste = db.poste.Find(id);
            if (poste == null)
            {
               return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Poste Not Found');
                         window.location.href='/Postes/Index'</script>");
            }
            var listligne = db.ligne.SqlQuery("SELECT CONCAT(ligne.Ligne,'_', atelier.Etage) AS Ligne1,ligne.Id,ligne.IdAtelier FROM ligne, atelier Where ligne.IdAtelier = atelier.Id").ToList();
            ViewBag.Idligne = new SelectList(listligne, "Id", "Ligne1", poste.Idligne);
            return View(poste);
        }

        // POST: Postes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Poste1,Idligne")] poste poste)
        {
            TempData["updated"] = null;

            if (ModelState.IsValid)
            {
                db.Entry(poste).State = EntityState.Modified;
                db.SaveChanges();
                TempData["updated"] = "updated";
            }
            else {
                var listligne = db.ligne.SqlQuery("SELECT CONCAT(ligne.Ligne,'_', atelier.Etage) AS Ligne1,ligne.Id,ligne.IdAtelier FROM ligne, atelier Where ligne.IdAtelier = atelier.Id").ToList();
                ViewBag.Idligne = new SelectList(listligne, "Id", "Ligne1");
                return View(poste);
            }
            return RedirectToAction("Index");
        }

        
        // POST: Postes/Delete/5
        [ ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            poste poste = db.poste.Find(id);
            db.poste.Remove(poste);
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
    }
}
