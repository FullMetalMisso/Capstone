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
            var existingCartItem = viniCart.FirstOrDefault(item => item.Vino.VinoId == vino.VinoId);
            if (existingCartItem != null)
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

            return RedirectToAction("Index", "Home"); // Reindirizzare direttamente al carrello? || incentivo all'acquisto?

        }

        [HttpGet]

        public ActionResult Cart()
        {
            //inizializzazione lista VinoCart
            List<VinoCart> userVinoCart = new List<VinoCart>();

            //verifica esistenza cookie carrello per l'utente loggato
            if (Request.Cookies["Carrello" + User.Identity.Name] != null && Request.Cookies["Carrello" + User.Identity.Name]["User"] != null)
            {
                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
                var userId = Convert.ToInt32(User.Identity.Name);

                //Decodifica val cookie & riempie lista
                var viniCart = JsonConvert.DeserializeObject<List<VinoCart>>(cartJson);

                //Filtra gli articoli in base all'utente loggato
                userVinoCart = viniCart.Where(v => v.UserId == userId).ToList();
                ViewBag.UserCart = userVinoCart;
            }
            return View();
        }

        [HttpPost]

        public ActionResult RimuoviDalCarrello(int vinoId)
        {
            var viniCart = new List<VinoCart>();
            if (Request.Cookies["Carrello" + User.Identity.Name] != null && Request.Cookies["Carrello" + User.Identity.Name]["User"] != null)
            {
                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
                viniCart = JsonConvert.DeserializeObject<List<VinoCart>>(cartJson);
            }

            var vinoRemove = viniCart.FirstOrDefault(v => v.Vino.VinoId == vinoId);
            if (vinoRemove != null)
            {
                viniCart.Remove(vinoRemove);
            }

            var cartCookie = new HttpCookie("Carrello" + User.Identity.Name);
            if(viniCart.Any()) //controlla la presenza di articoli nel carrello
            {
                cartCookie.Values["User"] = HttpUtility.UrlEncode(JsonConvert.SerializeObject(viniCart));
                cartCookie.Expires = DateTime.Now.AddDays(1);
            }
            else
            {
                cartCookie.Expires = DateTime.Now.AddDays(-1);
            }
            Response.Cookies.Add(cartCookie);

            return RedirectToAction("Cart");
          
        }

        [HttpPost]
        
        public ActionResult AggiornaQuantita(int vinoId, string operazione)
        {
            // Recupera l'elenco degli articoli attualmente nel carrello dal cookie
            var viniCart = new List<VinoCart>();
            if (Request.Cookies["Carrello" + User.Identity.Name] != null && Request.Cookies["Carrello" + User.Identity.Name]["User"] != null)
            {
                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
                viniCart = JsonConvert.DeserializeObject<List<VinoCart>>(cartJson);
            }

            // Trova l'articolo con l'ID specificato nell'elenco
            var vino = viniCart.FirstOrDefault(a => a.Vino.VinoId == vinoId);
            if (vino != null)
            {
                // Aggiorna la quantità in base all'operazione
                if (operazione == "incrementa")
                {
                    vino.Quantita++;
                }
                else if (operazione == "decrementa" && vino.Quantita > 1)
                {
                    vino.Quantita--;
                }
            }

            // Aggiorna il cookie del carrello con l'elenco aggiornato degli articoli
            var cartCookie = new HttpCookie("Carrello" + User.Identity.Name);
            cartCookie.Values["User"] = HttpUtility.UrlEncode(JsonConvert.SerializeObject(viniCart));
            cartCookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cartCookie);

            // Reindirizza alla vista del carrello aggiornata
            return RedirectToAction("Cart");
        }


        public ActionResult Search(string searchTerm)
        {
            // Esegui la ricerca nel database
           var vino = db.Vini.FirstOrDefault(v => v.Nome.Contains(searchTerm));

            if (vino != null)
            {
                // Se il vino è stato trovato, reindirizza alla pagina di dettaglio del vino
                return RedirectToAction("Dettaglio", "Home", new { id = vino.VinoId });
            }
            else
            {
                // Se il vino non è stato trovato, gestisci il caso in cui non ci siano corrispondenze
                ViewBag.ErrorMessage = "Nessun vino trovato.";
                return View("Error"); // Reindirizza a una vista di errore personalizzata
            }
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
