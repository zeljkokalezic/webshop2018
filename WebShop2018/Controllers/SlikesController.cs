using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebShop2018.Models;

namespace WebShop2018.Controllers
{
    public class SlikesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Slikes
        public ActionResult Index()
        {
            return View(db.Slike.ToList());
        }

        // GET: Slikes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slike slike = db.Slike.Find(id);
            if (slike == null)
            {
                return HttpNotFound();
            }
            return View(slike);
        }

        // GET: Slikes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Slikes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Naziv,Opis")] Slike slike)
        {
            if (ModelState.IsValid)
            {
                db.Slike.Add(slike);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(slike);
        }

        // GET: Slikes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slike slike = db.Slike.Find(id);
            if (slike == null)
            {
                return HttpNotFound();
            }
            return View(slike);
        }

        // POST: Slikes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Naziv,Opis")] Slike slike)
        {
            if (ModelState.IsValid)
            {
                db.Entry(slike).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(slike);
        }

        // GET: Slikes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slike slike = db.Slike.Find(id);
            if (slike == null)
            {
                return HttpNotFound();
            }
            return View(slike);
        }

        // POST: Slikes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Slike slike = db.Slike.Find(id);
            db.Slike.Remove(slike);
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
