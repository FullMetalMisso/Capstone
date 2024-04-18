using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Capstone.Controllers
{
    public class ProfiloController : Controller
    {
        private DBContext db = new DBContext();


        // GET: Profilo
        [Authorize] // Assicura che solo gli utenti autenticati possano accedere a questa azione
        public ActionResult ProfiloUtente()
        {
            // Recupera l'ID dell'utente attualmente autenticato
            int userId = GetCurrentUserId();

            // Cerca l'utente nel database
            Users user = db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Passa l'utente alla vista del profilo utente
            return View(user);
        }

        // Metodo per recuperare l'ID dell'utente attualmente autenticato
        private int GetCurrentUserId()
        {
            // Recupera l'ID dell'utente dall'identità corrente
            if (User.Identity.IsAuthenticated)
            {
                // Converti l'ID dell'utente memorizzato nel cookie di autenticazione in un intero
                return int.Parse(User.Identity.Name);
            }
            else
            {
                return 0; // L'utente non è autenticato, quindi non ha un ID valido
            }
        }

       
        // GET: Profilo/ModificaProfilo
        [Authorize]
        public ActionResult ModificaProfilo()
        {
            int userId = GetCurrentUserId();
            Users user = db.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Profilo/ModificaProfilo
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ModificaProfilo(Users user)
        {
            if (ModelState.IsValid)
            {
                // Recupera l'utente corrente dal database
                int userId = GetCurrentUserId();
                Users existingUser = db.Users.Find(userId);

                // Aggiorna solo i campi modificabili
                existingUser.Nome = user.Nome;
                existingUser.Cognome = user.Cognome;
                existingUser.Indirizzo = user.Indirizzo;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;

                // Mantieni il valore corrente del campo Ruolo
                existingUser.Ruolo = user.Ruolo;

                db.SaveChanges(); // Salva le modifiche nel database
                return RedirectToAction("ProfiloUtente");
            }
            return View(user);
        }
    }
}