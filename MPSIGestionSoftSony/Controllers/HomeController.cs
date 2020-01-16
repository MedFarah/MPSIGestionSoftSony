using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MPSIGestionSoftSony.Models;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using System.Security.Claims;
using System.Net;
using System.Collections;

namespace MPSIGestionSoftSony.Controllers
{
    public class HomeController : Controller
    {
        // private readonly userManager<ApplicationUser> userManager;
        GestionParcContext db = new GestionParcContext();
        String Role;
        // GET: HomeInstall-Package EntityFramework
        [Authorize/*(Roles = "dd,dfd")*/]
       public ActionResult Index()
        {

            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();

            var results = db.historiquepannehdd.ToList();
            //Select une seule date de mois
            var axis = results.Select(x => x.DatePH.Date.ToString("MMM-yyyy")).Distinct();
            
            foreach (var item in axis)
            {
                xValue.Add(item);
                yValue.Add(results.Count(x=>x.DatePH.Date.ToString("MMM-yyyy") == item));
            }
            ViewBag.X = xValue;
            ViewBag.Y = yValue;
            ViewBag.Equipement = db.equipement.Count();
            ViewBag.EquipementEnPanne = db.panneequipement.Count();
            ViewBag.HDD = db.hdd.Count();
            ViewBag.HDDenPanne = db.pannehdd.Count();
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            returnUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            try
            {
                // Verification.
                if (this.Request.IsAuthenticated && User.IsInRole("Admin") )
                {
                    // Info.

                    return Content(@"<script language='javascript' type='text/javascript'>
                         alert('Access Denied');
                         window.location.href='/Home/Index'</script>");
                }


            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.

            return this.View();
        }


        [HttpPost]
        [AllowAnonymous]
      
        public ActionResult Login(utilisateur model, string returnUrl)
        {
            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    // Initialization.
                    var loginInfo = this.db.utilisateur.Where(x => x.User == model.User.Trim() && x.Password == model.Password.Trim()).ToList();

                    // Verification.
                    if (loginInfo != null && loginInfo.Count() > 0)
                    {
                        // Initialization.
                        var logindetails = loginInfo.First();

                        // Login In.
                        this.SignInUser(logindetails.User, logindetails.Role, false);

                        // setting.
                        this.Session["role_id"] = logindetails.Role;
                        Session["Id"] = logindetails.Id;
                        Role = logindetails.Role;
                        // Info.
                        return this.RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Setting.

                        //ModelState.AddModelError("", "Invalid username or password");
                        string UrlPageAcc = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                        return Content(@"
                        <script language='javascript' type='text/javascript'>
                        alert('Username or password invalid!!!');
                        window.location.href='"+ UrlPageAcc+"'</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                // afficher erreur
                Console.Write(ex);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        public ActionResult LogOff()
        {
            try
            {

                var AuthenticationManager = HttpContext.GetOwinContext().Authentication;


                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                Session["role_id"] = null;
                AuthenticationManager.SignOut();
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
                FormsAuthentication.SignOut();
                // Sign Out.


            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }

            // Info.
            return this.RedirectToAction("Index", "Home");
        }



        private void SignInUser(string username, string role_id, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();

            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim(ClaimTypes.Role, role_id.ToString()));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;

                // Sign In.
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }



      

    }
}
