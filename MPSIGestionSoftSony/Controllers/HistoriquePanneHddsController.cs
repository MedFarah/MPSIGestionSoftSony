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
    [Authorize(Roles = "Admin")]
    [NoCache]
    public class HistoriquePanneHddsController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: HistoriquePanneHdds
        [Authorize]
        public ActionResult Index()
        {
            return View(db.historiquepannehdd.ToList());
        }

        // POST: HistoriquePanneHdds/Delete/5
        [ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            historiquepannehdd historiquepannehdd = db.historiquepannehdd.Find(id);
            db.historiquepannehdd.Remove(historiquepannehdd);
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
