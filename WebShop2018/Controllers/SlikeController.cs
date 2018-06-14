using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebShop2018.Models;

namespace WebShop2018.Controllers
{
    public class SlikeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: Slike
       [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Index()
        {
            return View(db.Slike.ToList());
        }

        // GET: Slike/Details/5
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slika slika = db.Slike.Find(id);
            if (slika == null)
            {
                return HttpNotFound();
            }
            return View(slika);
        }

        // GET: Slike/Create
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Slike/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Create(Slika slika)
        {
            if (ModelState.IsValid)
            {
                db.SaveChanges();

                return View("Index", "Artikli");
            }

            return RedirectToAction("Index");
        }

        // GET: Slike/Edit/5
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slika slika = db.Slike.Find(id);
            if (slika == null)
            {
                return HttpNotFound();
            }
            return View(slika);
        }

        // POST: Slike/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Edit([Bind(Include = "Id,NazivSlike,OpisSlike")] Slika slika, int? proizvodId)
        {
            if (ModelState.IsValid)
            {
                db.Entry(slika).State = EntityState.Modified;
                db.SaveChanges();

                if (proizvodId.HasValue)
                {
                    return RedirectToAction("Edit", "Artikli", new { id = proizvodId });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(slika);
        }

        // GET: Slike/Delete/5
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slika slika = db.Slike.Find(id);
            if (slika == null)
            {
                return HttpNotFound();
            }
            return View(slika);
        }

        // POST: Slike/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult DeleteConfirmed(int id, int? proizvodId)
        {
            Slika slika = db.Slike.Find(id);

            //brisemo najpre iz Content foldera
            var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{slika.NazivSlikeZaPrikaz}");

            if (System.IO.File.Exists(putanjaDoSlike))
            {
                System.IO.File.Delete(putanjaDoSlike);
            }

            db.Slike.Remove(slika);
            db.SaveChanges();

            if (proizvodId.HasValue)
            {
                return RedirectToAction("Edit", "Artikli", new { id = proizvodId });
            }
            else
            {
                return RedirectToAction("Index");
            }
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
