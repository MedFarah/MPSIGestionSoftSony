
using MPSIGestionSoftSony.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Globalization;
using MPSIGestionSoftSony.Utilities;


namespace MPSIGestionSoftSony.Controllers
{
    [Authorize/*(Roles = "test,test")*/]
    [NoCache]
    public class EquipementsController : Controller
    {
        GestionParcContext db = new GestionParcContext();

        // GET: Equipements
        [Authorize]
        [NoCache]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        [NoCache]
        public ActionResult Equipements()
        {
            var liste_poste = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
            ViewBag.poste_Id = new SelectList(liste_poste, "Id", "Poste1");
            TempData["url"] = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            
             return View();
        }

        //Ajouter equipement direct dans l'atelier
        public ActionResult AddEquipement(string Categorie)
        {
            ViewBag.cat = Categorie;
            var liste_poste = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
            ViewBag.poste_Id = new SelectList(liste_poste, "Id", "Poste1");
            //   TempData["url"] = url;
            return View("AddEquipement");
        }

        // GET: Equipements/Create
        [Authorize]
        public ActionResult Create(string Categorie)
        {
            ViewBag.cat = Categorie;
            var liste_poste = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
            ViewBag.poste_Id = new SelectList(liste_poste, "Id", "Poste1");
            return PartialView(new equipement());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NumSerie,Machine,Description,Application,Ip,Ram,Categorie,Modele,Marque,Dt_achat,IMMO,WinProdKey,System,SSID1,SSID2,Garantie,Etat,Dt_sortie,poste_Id")] equipement equipement, string NumSerie)
        {
            ViewBag.existdeja = null;
            
                var numseriexist = db.equipement.Where(x => x.NumSerie == NumSerie).Count();
                if (numseriexist == 0)
                {
                    db.equipement.Add(equipement);
                    db.SaveChanges();
                    TempData["success"] = "success";
                }
                else
                {
                    ViewBag.existdeja = "deja exist";
                    var liste_postee = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
                    ViewBag.poste_Id = new SelectList(liste_postee, "Id", "Poste1", equipement.poste_Id);

                    return View("AddEquipement");
                }
                //string url = TempData["url"].ToString();
                //// return Json(new { success = true, message = "operation effectue" }, JsonRequestBehavior.AllowGet);
                //return Redirect(url);
            

            var liste_poste = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
            ViewBag.poste_Id = new SelectList(liste_poste, "Id", "Poste1");
            //return PartialView("Create", equipement);
            // return View("Equipements");
            //return Redirect(TempData["url"].ToString());
            return RedirectToAction("AddEquipement", "Equipements", new
            {
                categorie = equipement.Categorie,
            });
        }


        // GET: Equipements/Edit/id
        [Authorize(Roles = "Admin,Technicien")]
        public ActionResult Edit(string id, string url)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            equipement equipement = db.equipement.Find(id);
            if (equipement == null)
            {
                return HttpNotFound();
            }
            var liste_poste = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
            ViewBag.poste_Id = new SelectList(liste_poste, "Id", "Poste1", equipement.poste_Id);
            ViewBag.Etat = new SelectList(db.equipement.GroupBy(x => x.Etat).Select(x => x.FirstOrDefault()), "Etat", "Etat", equipement.Etat);
            //  ViewBag.Categorie = new SelectList(db.equipement.GroupBy(x => x.Categorie).Select(x => x.FirstOrDefault()), "Categorie", "Categorie", equipement.Categorie);
            TempData["url"] = url;
            //  ViewBag.Categorie = new SelectList(db.equipement.GroupBy(x => x.Categorie).Select(x => x.FirstOrDefault()), "Categorie", "Categorie", equipement.Categorie);
            ViewBag.Active = equipement.Categorie;
            ViewBag.Etat = equipement.Etat;
            return View("EditEquipement", equipement);
        }

        // POST: Equipements/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Technicien")]
        public ActionResult Edit([Bind(Include = "NumSerie,Machine,Description,Application,Ip,Ram,Categorie,Modele,Marque,Dt_achat,IMMO,WinProdKey,System,SSID1,SSID2,Garantie,Etat,Dt_sortie,poste_Id")] equipement equipement)
        {
            TempData["name"] = null;

            if (ModelState.IsValid)
            {
                db.Entry(equipement).State = EntityState.Modified;
                db.SaveChanges();
                TempData["name"] = "equipement modifier";
            }
            else
            {
                var liste_poste = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
                ViewBag.poste_Id = new SelectList(liste_poste, "Id", "Poste1", equipement.poste_Id);
                return View(equipement);
            }
            if (TempData["url"] == null)
            { return RedirectToAction("Equipements"); } else { string url = TempData["url"].ToString(); return Redirect(url); }
            
        }

   

        [ActionName("Delete")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            equipement equipement = db.equipement.Where(x => x.NumSerie == id).FirstOrDefault();
            Backup(equipement);
            db.equipement.Remove(equipement);
            db.SaveChanges();
            if (TempData["url"] == null) { return RedirectToAction("Equipements"); } else { string url = TempData["url"].ToString(); return Redirect(url); }
            
        }

      
        public Boolean Backup(equipement equipement)
        {
            backup bac = new backup();
            bac.Application = equipement.Application;
            bac.Categorie = equipement.Categorie;
            bac.Description = equipement.Description;
            bac.Dt_achat = equipement.Dt_achat;
            bac.Dt_sortie = equipement.Dt_sortie;
            bac.Etat = equipement.Etat;
            bac.IMMO = equipement.IMMO;
            bac.Garantie = equipement.Garantie;
            bac.Ip = equipement.Ip;
            bac.Machine = equipement.Machine;
            bac.Marque = equipement.Marque;
            bac.Modele = equipement.Modele;
            bac.NumSerie = equipement.NumSerie;
            bac.Poste = equipement.poste_Id; 
            bac.WinProdKey = equipement.WinProdKey;
            bac.Ram = equipement.Ram;
            bac.SSID1 = equipement.SSID1;
            bac.SSID2 = equipement.SSID2;
            bac.System = equipement.System;
            BackupsController Bc = new BackupsController();
            Bc.Create(bac);
            //db.backup.Add(bac);
            
            // db.SaveChanges();
            return true;
        }

        [HttpPost]
        public ActionResult GetEquipements()
        {
            //d


            //string draw = Request.Form.GetValues("draw")[0];
            //string order = Request.Form.GetValues("order[0][column]")[0];
            //string orderDir = Request.Form.GetValues("order[0][dir]")[0];
            //int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            //int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
            var equipement = db.equipement.Include(e => e.poste).ToList();
            //Total record count.
            // int totalRecords = equipement.Count;
            // string search = Request.Form.GetValues("search[value]")[0];
            // // Filter record count.   
            // int recFilter = equipement.Count;
            // if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            // {
            //     // Apply search   
            //     equipement = db.equipement.Where(p => p.Description.ToString().ToLower().Contains(search.ToLower()) ||
            //          p.Categorie.ToLower().Contains(search.ToLower()) ||
            //          p.Modele.ToString().ToLower().Contains(search.ToLower()) ||
            //          p.System.ToLower().Contains(search.ToLower()) ||
            //          p.IMMO.ToLower().Contains(search.ToLower()) ||
            //          p.Ram.ToString().ToLower().Contains(search.ToLower()) ||
            //          p.Ip.ToString().ToLower().Contains(search.ToLower())).ToList();
            // }

            // //Sorting.
            //equipement = equipement.OrderBy(order + "" + orderDir).ToList();
            // // Apply pagination.   
            // equipement = equipement.Skip(startRec).Take(pageSize).ToList();

            var list = JsonConvert.SerializeObject(equipement,
    Formatting.None,
    new JsonSerializerSettings()
    {
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    });
            return Content(list, "application/json");
            //  return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

       
        [Authorize]
        [NoCache]
        public ActionResult AffichageParCategorieAndEtage(string Etage, string categorie)
        {
            TempData["url"] = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            
            ViewBag.Active = categorie;
            ViewBag.Etage = categorie + Etage;
            ViewBag.Nom = categorie + " " + Etage;
            var equipement = db.equipement.SqlQuery("SELECT * FROM equipement, atelier, ligne, poste where  equipement.poste_Id = poste.Id and poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id and atelier.Etage ='" + Etage + "' and equipement.Categorie='" + categorie + "'").ToList();
            return View("EquipementsByAtelier", equipement);
        }
        [Authorize]
        [NoCache]
        public ActionResult AffichageParCategorie(string categorie)
        {
            TempData["url"] = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            ViewBag.Active = categorie;
            ViewBag.ActiveE = categorie;
            ViewBag.Nom = "All " + categorie;
            var equipement = db.equipement.SqlQuery("SELECT * FROM equipement where equipement.Categorie='" + categorie + "'").ToList();
            return View("EquipementsByAtelier", equipement);
        }
        [Authorize]
        [NoCache]
        public ActionResult AffichageParEtat(string categorie, string Etat)
        {
            TempData["url"] = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            ViewBag.Active = categorie;
            ViewBag.current = categorie + Etat;
            ViewBag.Cat = categorie;
            ViewBag.Nom = categorie + " " + Etat;
            var equipement = db.equipement.SqlQuery("SELECT * FROM equipement where equipement.Categorie='" + categorie + "' and equipement.Etat='" + Etat + "'").ToList();
            return View("EquipementsByAtelier", equipement);
        }
        [Authorize]
        [NoCache]
        public ActionResult AffichageParCategorieAndEtageAndLigne(string Etage, string categorie, string Ligne)
        {
            ViewBag.Active = categorie;
            ViewBag.Etage = categorie + Etage;
            ViewBag.current = categorie + Ligne;

            TempData["url"] = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;

            ViewBag.Nom = categorie.ToUpper() + " " + Ligne + " " + Etage;

            var equipement = db.equipement.SqlQuery("SELECT * FROM equipement, atelier, ligne, poste where  equipement.poste_Id = poste.Id and poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id and atelier.Etage ='" + Etage + "' and equipement.Categorie='" + categorie + "' and ligne.Ligne='" + Ligne + "'").ToList();
            return View("EquipementsByAtelier", equipement);

        }

    

        //Import Excel
        [Authorize(Roles = "Admin,Technicien")]
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Please select a excel file<br>";
                var liste_poste = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
                ViewBag.poste_Id = new SelectList(liste_poste, "Id", "Poste1");
                return View("AddEquipement");
            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    string path = Server.MapPath("~/Content/" + excelfile.FileName);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    excelfile.SaveAs(path);
                    // Read data from excel file
                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    List<equipement> listEquipements = new List<equipement>();
                    string h1 = ((Excel.Range)range.Cells[2, 1]).Text;
                    string h2 = ((Excel.Range)range.Cells[2, 2]).Text;
                    string h3 = ((Excel.Range)range.Cells[2, 3]).Text;
                    string h4 = ((Excel.Range)range.Cells[2, 4]).Text;
                    string h5 = ((Excel.Range)range.Cells[2, 5]).Text;
                    string h6 = ((Excel.Range)range.Cells[2, 6]).Text;
                    string h7 = ((Excel.Range)range.Cells[2, 7]).Text;
                    string h8 = ((Excel.Range)range.Cells[2, 8]).Text;
                    string h9 = ((Excel.Range)range.Cells[2, 9]).Text;
                    string h10 = ((Excel.Range)range.Cells[2, 10]).Text;
                    string h11 = ((Excel.Range)range.Cells[2, 11]).Text;
                    string h12 = ((Excel.Range)range.Cells[2, 12]).Text;
                    string h13 = ((Excel.Range)range.Cells[2, 13]).Text;
                    string h14 = ((Excel.Range)range.Cells[2, 14]).Text;

                    //test header
                    if ("NumSerie" == h1 && "Description" == h2 && "Application" == h3 &&  "Ip" == h4 && "Ram" == h5 &&
                     "categorie" == h6 && "Model" == h7 && "Dt_achat" == h8 &&  "IMMO" == h9 && "WinProdKey" == h10 &&
                     "System" == h11 && "Etat" == h12 && "Dt_sortie" == h13 && "Poste" == h14)
                    {
                        for (int row = 3; row <= range.Rows.Count; row++)
                        {

                            equipement e = new equipement();
                            e.NumSerie = ((Excel.Range)range.Cells[row, 1]).Text;
                            e.Description = ((Excel.Range)range.Cells[row, 2]).Text;
                            e.Application = ((Excel.Range)range.Cells[row, 3]).Text;
                            e.Ip = ((Excel.Range)range.Cells[row, 4]).Text;
                            e.Ram = Convert.ToInt32(((Excel.Range)range.Cells[row, 5]).Text);
                            e.Categorie = ((Excel.Range)range.Cells[row, 6]).Text;
                            e.Modele = ((Excel.Range)range.Cells[row, 7]).Text;
                            var formatStrings = new string[] { "MM/dd/yyyy", "yyyy-MM-dd", "M/dd/yyyy", "MM/d/yyyy" };
                            DateTime dt = DateTime.ParseExact(((Excel.Range)range.Cells[row, 8]).Text, formatStrings, CultureInfo.InvariantCulture, DateTimeStyles.None);
                            // DateTime Dt_achat = Convert.ToDateTime(((Excel.Range)range.Cells[row, 8]).Text);
                            e.Dt_achat = dt;
                            e.IMMO = ((Excel.Range)range.Cells[row, 9]).Text;
                            e.WinProdKey = ((Excel.Range)range.Cells[row, 10]).Text;
                            e.System = ((Excel.Range)range.Cells[row, 11]).Text;
                            e.Etat = ((Excel.Range)range.Cells[row, 12]).Text;
                            DateTime dtt = DateTime.ParseExact(((Excel.Range)range.Cells[row, 13]).Text, formatStrings, CultureInfo.InvariantCulture, DateTimeStyles.None);
                            //DateTime Dt_sortie = Convert.ToDateTime(((Excel.Range)range.Cells[row, 13]).Text);
                            e.Dt_sortie = dtt;
                            string Nposte = ((Excel.Range)range.Cells[row, 14]).Text;
                            var x = db.poste.Where(y => y.Poste1 == Nposte).FirstOrDefault();
                            if (x == null) { e.poste_Id = null; e.poste = null; }
                            else {
                                e.poste_Id = x.Id;
                                e.poste = x;
                            }
                            
                            db.equipement.Add(e);
                            db.SaveChanges();
                            TempData["Done"] = "Done";
                            //listEquipements.Add(e);
                        }
                        //ViewBag.ListProducts = listProducts;
                        return RedirectToAction("Equipements", "Equipements");

                    }
                    else
                    {
                        ViewBag.Format = "File type is incorrect<br>";
                        var liste_poste = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
                        ViewBag.poste_Id = new SelectList(liste_poste, "Id", "Poste1");
                        return View("AddEquipement");

                    }
                   
                }
                else
                {
                    ViewBag.Error = "File type is incorrect<br>";
                    var liste_poste = db.poste.SqlQuery("SELECT CONCAT(poste.Poste,'__',ligne.Ligne,'__', atelier.Etage) AS Poste1,poste.Id,poste.Idligne FROM poste, ligne, atelier Where poste.Idligne = ligne.Id and ligne.IdAtelier = atelier.Id").ToList();
                    ViewBag.poste_Id = new SelectList(liste_poste, "Id", "Poste1");
                    return View("AddEquipement");
                }

            }

        }
    }

    

}