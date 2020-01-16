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
    public class BackupsController : Controller
    {
        private GestionParcContext db = new GestionParcContext();

        // GET: Backups
        public ActionResult Index()
        {
            return View(db.backup.ToList());
        }

    

        // GET: Backups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Backups/Create
         [HttpPost]
        public ActionResult Create([Bind(Include = "Id,NumSerie,Description,Application,Ip,Ram,Categorie,Modele,Marque,Dt_achat,IMMO,WinProdKey,System,Dt_sortie,Etat,SSID1,SSID2,Machine,Garantie,Poste")] backup backup)
        {
            
                db.backup.Add(backup);
                db.SaveChanges();
                return RedirectToAction("Index");
            

        }

       


        // POST: Backups/Restore
        [HttpPost]
        public ActionResult Restore([Bind(Include = "Id,NumSerie,Description,Application,Ip,Ram,Categorie,Modele,Marque,Dt_achat,IMMO,WinProdKey,System,Dt_sortie,Etat,SSID1,SSID2,Machine,Garantie,Poste")] backup backup,int id)
        {
            backup backupRes = db.backup.Find(id);
            if (backupRes == null)
            {
                return HttpNotFound();
            }
            else
            {

                equipement e = new equipement();
                e.NumSerie = backupRes.NumSerie;
                e.Description = backupRes.Description;
                e.Application = backupRes.Application;
                e.Ip = backupRes.Ip;
                e.Ram = backupRes.Ram;
                e.Categorie = backupRes.Categorie;
                e.Modele = backupRes.Modele;
                e.Dt_achat = backupRes.Dt_achat;
                e.IMMO = backupRes.IMMO;
                e.WinProdKey = backupRes.WinProdKey;
                e.System = backupRes.System;
                e.Etat = backupRes.Etat;
                e.Dt_sortie = backupRes.Dt_sortie;
                e.poste_Id = backupRes.Poste;
                EquipementsController Con = new EquipementsController();
                Con.Create(e, backup.NumSerie);
                db.backup.Remove(backupRes);
                db.SaveChanges();
            }
            return View(backup);
        }

      

        // POST: Backups/Delete/
        [ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            backup backup = db.backup.Find(id);
            db.backup.Remove(backup);
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
