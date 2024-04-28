using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;

namespace Capstone.Controllers
{
    public class OrdiniController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Ordini
        [Authorize(Roles = "Amministratore")]
        public ActionResult Index()
        {
            var ordini = db.Ordini.Include(o => o.Pagamenti).Include(o => o.Users);
            return View(ordini.ToList());
        }

        // GET: Ordini/Details/5
        [Authorize(Roles = "Cliente, Amministratore")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var idInt = Convert.ToInt32(id);
            var orders = db.Ordini
                         .Where(o => o.UserId == idInt)
                         .OrderByDescending(o => o.OrdiniId) // Ordina per ID in modo decrescente
                         .ToList();

            if (orders == null || orders.Count == 0)
            {
                return HttpNotFound();
            }

            return View(orders);
        }

        // GET: Ordini/Create 

        //public ActionResult Create()
        //{
        //    ViewBag.PagamentoId = new SelectList(db.Pagamenti, "PagamentoId", "TipoPagamento");
        //    ViewBag.UserId = new SelectList(db.Users, "UserId", "Nome");
        //    return View();
        //}

        //// POST: Ordini/Create
        //// Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        //// Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "OrdiniId,Indirizzo,Stato,Totale,Consegna,UserId,Nome,Cognome,PagamentoId")] Ordini ordini)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Ordini.Add(ordini);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.PagamentoId = new SelectList(db.Pagamenti, "PagamentoId", "TipoPagamento", ordini.PagamentoId);
        //    ViewBag.UserId = new SelectList(db.Users, "UserId", "Nome", ordini.UserId);
        //    return View(ordini);
        //}

        // GET: Ordini/Edit/5
        [Authorize(Roles = "Amministratore")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            ViewBag.PagamentoId = new SelectList(db.Pagamenti, "PagamentoId", "TipoPagamento", ordini.PagamentoId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Nome", ordini.UserId);
            return View(ordini);
        }

        // POST: Ordini/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore")]
        public ActionResult Edit(int id, string stato, DateTime consegna)
        {
            var ordine = db.Ordini.Find(id);
            if (ordine == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                // Imposta manualmente solo Stato e Consegna
                ordine.Stato = stato;
                ordine.Consegna = consegna;

                db.Entry(ordine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Se il modello non è valido, ricarica le liste e restituisci la vista con l'ordine
            ViewBag.PagamentoId = new SelectList(db.Pagamenti, "PagamentoId", "TipoPagamento", ordine.PagamentoId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Nome", ordine.UserId);
            return View(ordine);
        }

        // GET: Ordini/Delete/5
        [Authorize(Roles = "Amministratore")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        // POST: Ordini/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ordini ordini = db.Ordini.Find(id);
            db.Ordini.Remove(ordini);
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
