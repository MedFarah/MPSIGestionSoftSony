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
using Newtonsoft.Json;

namespace MPSIGestionSoftSony.Controllers
{
    [Authorize]
    [NoCache]
    public class LignesController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: Lignes
        [Authorize]
        public ActionResult Index()
        {
            var ligne = db.ligne.Include(l => l.atelier);
            ViewBag.IdAtelier = new SelectList(db.atelier, "Id", "Etage");
            return View(ligne.ToList());
        }



        // GET: Lignes/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.IdAtelier = new SelectList(db.atelier, "Id", "Etage");
            return View();
        }

        // POST: Lignes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ligne1,IdAtelier")] ligne ligne)
        {
            if (ModelState.IsValid)
            {
                db.ligne.Add(ligne);
                db.SaveChanges();
                TempData["created"] = "ligne created";
                return RedirectToAction("Index");
            }

            ViewBag.IdAtelier = new SelectList(db.atelier, "Id", "Etage", ligne.IdAtelier);
            return View(ligne);
        }

        // GET: Lignes/Edit/id
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ligne ligne = db.ligne.Find(id);
            if (ligne == null)
            {
                return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Line Not Found');
                         window.location.href='/Lignes/Index'</script>");
            }
            ViewBag.IdAtelier = new SelectList(db.atelier, "Id", "Etage", ligne.IdAtelier);
            return View(ligne);
        }

        // POST: Lignes/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ligne1,IdAtelier")] ligne ligne)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ligne).State = EntityState.Modified;
                db.SaveChanges();
                TempData["name"] = "ligne modifier";
                return RedirectToAction("Index");
            }
            ViewBag.IdAtelier = new SelectList(db.atelier, "Id", "Etage", ligne.IdAtelier);
            return View(ligne);
        }

        
        // POST: Lignes/Delete/
        [ ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            ligne ligne = db.ligne.Find(id);
            db.ligne.Remove(ligne);
            db.SaveChanges();
            TempData["delete"] = "Line deleted";
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
