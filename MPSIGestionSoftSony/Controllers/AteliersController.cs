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
    [NoCache]
    public class AteliersController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: Ateliers
        public ActionResult Index()
        {
            return View(db.atelier.ToList());
        }


        // GET: Ateliers/Create
        
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ateliers/Create
          [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Etage")] atelier atelier)
        {
            if (ModelState.IsValid)
            {
                db.atelier.Add(atelier);
                db.SaveChanges();
                TempData["created"] = "atelier created";
                return RedirectToAction("Index");
            }

            return View(atelier);
        }

        // GET: Ateliers/Edit/id
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            atelier atelier = db.atelier.Find(id);
            if (atelier == null)
            {
                return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Etage Not Found');
                         window.location.href='/Ateliers/Index'</script>"); 
            }
            return View(atelier);
        }

        // POST: Ateliers/Edit/id
          [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Etage")] atelier atelier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atelier).State = EntityState.Modified;
                db.SaveChanges();
                TempData["name"] = "atelier modifier";
                return RedirectToAction("Index");
            }
            return View(atelier);
        }

       
        // POST: Ateliers/Delete/id
        [ ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            atelier atelier = db.atelier.Find(id);
            db.atelier.Remove(atelier);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        // libérer les ressources internes
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
