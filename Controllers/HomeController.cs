using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;

namespace Capstone.Controllers
{
    public class HomeController : Controller
    {
        private DBContext db = new DBContext();
       
        //GET: Lista Vini
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View(db.Vini.ToList());
        }

        //GET: 
        public ActionResult Dettaglio(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Vini vino = db.Vini.Find(id);

            if (vino == null)
            {
                return HttpNotFound();
            }

            return View(vino);
        }



    }
}
