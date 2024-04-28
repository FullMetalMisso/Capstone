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
    public class VinisController : Controller
    {
        private DBContext db = new DBContext();

     


        // GET: Vinis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vini vini = db.Vini.Find(id);
            if (vini == null)
            {
                return HttpNotFound();
            }
            return View(vini);
        }

        // GET: Vinis/Create
        [Authorize(Roles = "Amministratore")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: Vinis/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore")]
        public ActionResult Create([Bind(Include = "VinoId,Nome,Img,Tipo,Anno,Descrizione,Prezzo,Magazzino,Produttore,SottoTipo,Sconto")] Vini vini)
        {
            if (ModelState.IsValid)
            {
                db.Vini.Add(vini);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vini);
        }

        // GET: Vinis/Edit/5
        [Authorize(Roles = "Amministratore")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vini vini = db.Vini.Find(id);
            if (vini == null)
            {
                return HttpNotFound();
            }
            return View(vini);
        }

        // POST: Vinis/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore")]
        public ActionResult Edit([Bind(Include = "VinoId,Nome,Img,Tipo,Anno,Descrizione,Prezzo,Magazzino,Produttore,SottoTipo,Sconto")] Vini vini)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vini).State = EntityState.Modified;
                db.SaveChanges();
                // Reindirizza l'utente alla pagina Index del controller Home
                return RedirectToAction("Index", "Home");
            }
            return View(vini);
        }

        // GET: Vinis/Delete/5
        [Authorize(Roles = "Amministratore")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vini vini = db.Vini.Find(id);
            if (vini == null)
            {
                return HttpNotFound();
            }
            return View(vini);
        }

        // POST: Vinis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministratore")]
        public ActionResult DeleteConfirmed(int id)
        {
            Vini vini = db.Vini.Find(id);
            db.Vini.Remove(vini);
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
