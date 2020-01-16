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
    public class PannesController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: Pannes
        [Authorize]
        public ActionResult Index()
        {
            return View(db.panne.ToList());
        }


        // GET: Pannes/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pannes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NomPanne")] panne panne)
        {
            if (ModelState.IsValid)
            {
                db.panne.Add(panne);
                db.SaveChanges();
                TempData["created"] = "Panne created";
                return RedirectToAction("Index");
            }

            return View(panne);
        }

        // GET: Pannes/Edit/
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            panne panne = db.panne.Find(id);
            if (panne == null)
            {
                return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Panne Not Found');
                         window.location.href='/Pannes/Index'</script>");
            }
            return View(panne);
        }

        // POST: Pannes/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NomPanne")] panne panne)
        {
            if (ModelState.IsValid)
            {
                db.Entry(panne).State = EntityState.Modified;
                db.SaveChanges();
                TempData["updated"] = "panne modifier";
                return RedirectToAction("Index");
            }
            return View(panne);
        }

         // POST: Pannes/Delete/
        [ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            panne panne = db.panne.Find(id);
            db.panne.Remove(panne);
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
