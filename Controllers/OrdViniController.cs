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

        [Authorize(Roles = "Amministratore")]
        public ActionResult Index()
        {
            var ordArt = db.OrdVini.Include(o => o.Vini).Include(o => o.Ordini);
            return View(ordArt.ToList());
        }
        // GET: OrdVini
        [Authorize(Roles = "Amministratore,Cliente")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ordineWithArticoli = db.OrdVini
                .Include(o => o.Ordini)
                .Include(o => o.Ordini.Users)
                .Include(o => o.Vini)
                .Where(o => o.OrdiniId == id).ToList();

            if (ordineWithArticoli == null)
            {
                return HttpNotFound();
            }

            return View(ordineWithArticoli);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
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
                        Prezzo = vino.PrezzoConSconto,
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
        [Authorize(Roles = "Cliente")]
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
                ViewBag.Pagamento = new SelectList(db.Pagamenti, "PagamentoId", "TipoPagamento");
            }
            // Ottieni il numero di elementi nel carrello
            int numeroElementiCarrello = userVinoCart.Sum(item => item.Quantita);
            ViewBag.NumeroElementiCarrello = numeroElementiCarrello;
            return View();
        }

        public ActionResult GetNumeroElementiCarrello()
        {
            // Inizializza il numero di elementi nel carrello a 0
            int numeroElementiCarrello = 0;

            // Verifica se esiste un cookie per il carrello per l'utente attualmente autenticato
            if (Request.Cookies["Carrello" + User.Identity.Name] != null && Request.Cookies["Carrello" + User.Identity.Name]["User"] != null)
            {
                // Decodifica il valore del cookie per ottenere la lista dei prodotti nel carrello
                var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);

                // Deserializza la lista dei prodotti nel carrello
                var viniCart = JsonConvert.DeserializeObject<List<VinoCart>>(cartJson);

                // Somma le quantità di tutti i prodotti nel carrello
                numeroElementiCarrello = viniCart.Sum(item => item.Quantita);
            }

            // Restituisci il numero di elementi nel carrello in formato JSON
            return Json(numeroElementiCarrello, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize(Roles = "Cliente")]
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
        [Authorize(Roles = "Cliente")]
        [ValidateAntiForgeryToken]

        public ActionResult CreateOrderFromCart(Ordini ordVino)
        {
            // Imposta l'ID dell'utente per l'ordine
            ordVino.UserId = Convert.ToInt32(User.Identity.Name);

            // Recupera il carrello degli acquisti dell'utente corrente
            var cartJson = HttpUtility.UrlDecode(Request.Cookies["Carrello" + User.Identity.Name]["User"]);
            var viniCart = JsonConvert.DeserializeObject<List<VinoCart>>(cartJson);

            // Calcola il totale dell'ordine e aggiorna il magazzino
            decimal totale = 0;
            foreach (var vino in viniCart.Where(a => a.UserId == ordVino.UserId))
            {
                totale += (vino.Quantita * vino.Vino.Prezzo);
                var vinoAcquistato = db.Vini.Find(vino.Vino.VinoId);
                if (vinoAcquistato != null)
                {
                    vinoAcquistato.Magazzino -= vino.Quantita;
                }
            }

            // Imposta lo stato e il totale dell'ordine
            ordVino.Stato = "In Preparazione";
            ordVino.Totale = totale;

           
            db.Ordini.Add(ordVino);
            db.SaveChanges();

            // Ottiene l'ID dell'ordine appena creato
            int newOrdineID = ordVino.OrdiniId;

            // Aggiunge i dettagli dell'ordine nel database
            foreach (var vino in viniCart.Where(a => a.UserId == ordVino.UserId))
            {
                var newOrdVino = new OrdVini
                {
                    VinoId = vino.Vino.VinoId,
                    OrdiniId = newOrdineID,
                    Quantita = Convert.ToInt32(vino.Quantita)
                };
                db.OrdVini.Add(newOrdVino);
            }

            
            db.SaveChanges();

            // Svuota il carrello dell'utente cancellando i cookie
            HttpCookie userCookie = Request.Cookies["Carrello" + User.Identity.Name];
            if (userCookie != null)
            {
                userCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userCookie);
            }

            
            return RedirectToAction("Details", "OrdVini", new { id = newOrdineID });
        }



        [HttpPost]
        [Authorize(Roles = "Cliente")]
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

        public ActionResult ViniPerTipo(string tipo, string sottoTipo)
        {
            // Recupera i vini dal database basandoti sul tipo e sottoTipo specificati
            var viniPerTipo = db.Vini.Where(v => v.Tipo == tipo && (sottoTipo == null || v.SottoTipo == sottoTipo)).ToList();

            // Passa il tipo di vino, il sottoTipo e i vini alla vista per la visualizzazione
            ViewBag.TipoVino = tipo;
            ViewBag.SottoTipoVino = sottoTipo;
            return View(viniPerTipo);
        }

        // GET: OrdVini/Delete/5
        [Authorize(Roles = "Amministratore,Cliente")]
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
        [Authorize(Roles = "Amministratore")]
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
