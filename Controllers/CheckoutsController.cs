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
    public class CheckoutsController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Checkouts
        public ActionResult Index()
        {
            var checkout = db.Checkout.Include(c => c.Ordini).Include(c => c.Users);
            return View(checkout.ToList());
        }

        // GET: Checkouts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Checkout checkout = db.Checkout.Find(id);
            if (checkout == null)
            {
                return HttpNotFound();
            }
            return View(checkout);
        }

        // GET: Checkouts/Create
        public ActionResult Create()
        {
            ViewBag.OrdiniId = new SelectList(db.Ordini, "OrdiniId", "Indirizzo");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Nome");
            return View();
        }

        // POST: Checkouts/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckoutId,UserId,DataCheckout,Totale,OrdiniId")] Checkout checkout)
        {
            if (ModelState.IsValid)
            {
                db.Checkout.Add(checkout);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrdiniId = new SelectList(db.Ordini, "OrdiniId", "Indirizzo", checkout.OrdiniId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Nome", checkout.UserId);
            return View(checkout);
        }

        // GET: Checkouts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Checkout checkout = db.Checkout.Find(id);
            if (checkout == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrdiniId = new SelectList(db.Ordini, "OrdiniId", "Indirizzo", checkout.OrdiniId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Nome", checkout.UserId);
            return View(checkout);
        }

        // POST: Checkouts/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CheckoutId,UserId,DataCheckout,Totale,OrdiniId")] Checkout checkout)
        {
            if (ModelState.IsValid)
            {
                db.Entry(checkout).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrdiniId = new SelectList(db.Ordini, "OrdiniId", "Indirizzo", checkout.OrdiniId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Nome", checkout.UserId);
            return View(checkout);
        }

        // GET: Checkouts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Checkout checkout = db.Checkout.Find(id);
            if (checkout == null)
            {
                return HttpNotFound();
            }
            return View(checkout);
        }

        // POST: Checkouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Checkout checkout = db.Checkout.Find(id);
            db.Checkout.Remove(checkout);
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
