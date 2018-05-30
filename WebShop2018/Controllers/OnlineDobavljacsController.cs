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
    public class OnlineDobavljacsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OnlineDobavljacs
        public ActionResult Index()
        {
            return View(db.Dobavljacs.ToList());
        }

        // GET: OnlineDobavljacs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineDobavljac onlineDobavljac = db.Dobavljacs.Find(id);
            if (onlineDobavljac == null)
            {
                return HttpNotFound();
            }
            return View(onlineDobavljac);
        }

        // GET: OnlineDobavljacs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OnlineDobavljacs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ime,Aktivan,Email,WebsiteURL")] OnlineDobavljac onlineDobavljac)
        {
            if (ModelState.IsValid)
            {
                db.Dobavljacs.Add(onlineDobavljac);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(onlineDobavljac);
        }

        // GET: OnlineDobavljacs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineDobavljac onlineDobavljac = db.Dobavljacs.Find(id);
            if (onlineDobavljac == null)
            {
                return HttpNotFound();
            }
            return View(onlineDobavljac);
        }

        // POST: OnlineDobavljacs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ime,Aktivan,Email,WebsiteURL")] OnlineDobavljac onlineDobavljac)
        {
            if (ModelState.IsValid)
            {
                db.Entry(onlineDobavljac).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(onlineDobavljac);
        }

        // GET: OnlineDobavljacs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineDobavljac onlineDobavljac = db.Dobavljacs.Find(id);
            if (onlineDobavljac == null)
            {
                return HttpNotFound();
            }
            return View(onlineDobavljac);
        }

        // POST: OnlineDobavljacs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OnlineDobavljac onlineDobavljac = db.Dobavljacs.Find(id);
            db.Dobavljacs.Remove(onlineDobavljac);
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
