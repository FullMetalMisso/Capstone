using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using Newtonsoft.Json;

namespace Capstone.Controllers
{
    public class OrdViniController : Controller
    {
        private DBContext db = new DBContext();

        // GET: OrdVini
        public ActionResult Index()
        {
            return View(db.OrdVini.ToList());
        }

        // GET: OrdVini/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrdVini ordVini = db.OrdVini.Find(id);
            if (ordVini == null)
            {
                return HttpNotFound();
            }
            return View(ordVini);
        }

        [HttpPost]
        public ActionResult AddToCart(int? id)
        {
            //controllo id
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //controllo esistenza Vino
            var vino = db.Vini.FirstOrDefault(v => v.VinoId == id);
            if (vino == null)
            {
                return HttpNotFound();
            }

            //Inizializzazione lista vuota
            List<VinoCart> viniCart = new List<VinoCart>();

            //cookie e controlli
            HttpCookie cartCookie;

            //nel caso sia giá esistente i dati vengono recuperati e deserializzati
            if (Request.Cookies["Carrello" + User.Identity.Name] != null && Request.Cookies["Carrello" + User.Identity.Name]["User"] != null)
            {
                cartCookie = Request.Cookies["Carrello" + User.Identity.Name];
                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
                viniCart = JsonConvert.DeserializeObject<List<VinoCart>>(cartJson);
            } else
            {
                //altrimenti si crea il cookie e si assegna all'utente
                cartCookie = new HttpCookie("Carrello" + User.Identity.Name);
                cartCookie.Values["User"] = User.Identity.Name; 
            }
            // Se il vino era giá nel carello viene semplicemente incrementato
            var existingCartItem = viniCart.FirstOrDefault(v => v.Vino.VinoId == vino.VinoId);
            if (existingCartItem == null)
            {
                existingCartItem.Quantita++;
            }
            else
            {
                //altrimenti crezione nuovo oggetto e aggiunto a vinoCart
                var vinoCart = new VinoCart()
                {
                    Vino = new Vini()
                    {
                        VinoId = vino.VinoId,
                        Nome = vino.Nome,
                        Prezzo = vino.Prezzo,
                        Img = vino.Img,
                        Tipo = vino.Tipo,
                        Anno = vino.Anno,
                        Descrizione = vino.Descrizione,
                        Magazzino = vino.Magazzino, //Ridurre qua il numero in magazzino? Fattibile? Senso? 
                        Produttore = vino.Produttore,
                    },
                    Quantita = 1,
                    UserId = Convert.ToInt32(User.Identity.Name),
                };
                viniCart.Add(vinoCart);
            }
            //si serializza la lista
            cartCookie["User"] = HttpUtility.UrlEncode(JsonConvert.SerializeObject(viniCart));
         
            //scadenza del cookie ad un giorno
            cartCookie.Expires = DateTime.Now.AddDays(1); // 1 giorno o piú?

            //Cookie aggiunto alla response e finally reindirizzazione alla home. 
            Response.Cookies.Add(cartCookie);

            return RedirectToAction("Index"); // Reindirizzare direttamente al carrello? || incentivo all'acquisto?

        }

       
        // GET: OrdVini/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrdVini ordVini = db.OrdVini.Find(id);
            if (ordVini == null)
            {
                return HttpNotFound();
            }
            return View(ordVini);
        }

        // POST: OrdVini/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrdVini ordVini = db.OrdVini.Find(id);
            db.OrdVini.Remove(ordVini);
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
