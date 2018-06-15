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
    public class PicturesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pictures
        public ActionResult Index()
        {            
             return View(db.Proizvodi.ToList());
        }

        // GET: Pictures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var picture = db.Pictures.Where(p => p.Proizvod.Id == id);
            // Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // GET: Pictures/Create
        public ActionResult Create()
        {
            PostaviProizvode();
            return View();
        }

        // POST: Pictures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NazivSlike,Opis,Proizvod")] Picture picture, HttpPostedFileBase slikica)
        {
            if (ModelState.IsValid)
            {
                picture.Proizvod = db.Proizvodi.Find(picture.Proizvod.Id);
                db.Pictures.Add(picture);
                db.SaveChanges();

                SnimiSlikuIDodeliNazivSlike(picture, slikica);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            PostaviProizvode();
            return View(picture);
        }

        private void SnimiSlikuIDodeliNazivSlike(Picture picture, HttpPostedFileBase slikica)
        {
            if (slikica != null)
            {
                picture.NazivSlike = Path.GetFileName(slikica.FileName);

                var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{picture.NazivSlikeZaPrikaz}");
                slikica.SaveAs(putanjaDoSlike);
            }
        }

        // GET: Pictures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            PostaviProizvode();
            return View(picture);
        }

        // POST: Pictures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NazivSlike,Opis")] Picture picture)
        {
            if (ModelState.IsValid)
            {
                db.Entry(picture).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index"); //Ne moze da mi vrati na details zato sto nema ID tog proizvoda kad vraca
            }
            PostaviProizvode();
            return View(picture);
        }

        // GET: Pictures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        public void PostaviProizvode()
        {
            ViewBag.Proizvodi = db.Proizvodi.ToList();
        }

        // POST: Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Picture picture = db.Pictures.Find(id);
            db.Pictures.Remove(picture);
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
