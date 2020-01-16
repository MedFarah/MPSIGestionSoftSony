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
    public class HistoriquePanneEquipementsController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: HistoriquePanneEquipements
        [Authorize]
        public ActionResult Index()
        {
            return View(db.historiquepanneequipement.ToList());
        }

      

        // POST: HistoriquePanneEquipements/Delete/id
        [ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            historiquepanneequipement historiquepanneequipement = db.historiquepanneequipement.Find(id);
            db.historiquepanneequipement.Remove(historiquepanneequipement);
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
