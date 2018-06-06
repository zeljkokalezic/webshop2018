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
    [Authorize (Roles = RolesConfig.USER)]
    public class ProizvodiController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Proizvodi
        [AllowAnonymous]
        public ActionResult Index()
        {
            //var proizvodi = db.Proizvodi.Where(p => p.Cena < 100)
            //    .OrderByDescending(x => x.Cena)
            //    .ToList();

            //return View(proizvodi);

            return View(db.Proizvodi.ToList());
        }

        public ActionResult BootstrapIndex()
        {
            //var proizvodi = db.Proizvodi.Where(p => p.Cena < 100)
            //    .OrderByDescending(x => x.Cena)
            //    .ToList();

            //return View(proizvodi);

            return View(db.Proizvodi.ToList());
        }

        // GET: Proizvodi/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proizvod proizvod = db.Proizvodi.Where(p => p.Id == id).FirstOrDefault();
            if (proizvod == null)
            {
                return HttpNotFound();
            }
            return View(proizvod);
        }

        // GET: Proizvodi/Create
        public ActionResult Create()
        {
            PopuniKategorije();
            return View();
        }

        // POST: Proizvodi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Naziv,Cena,Stanje,Kategorija")] Proizvod proizvod)
        {
            if (ModelState.IsValid)
            {
                proizvod.Kategorija = db.Kategorije.Find(proizvod.Kategorija.Id);
                db.Proizvodi.Add(proizvod);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopuniKategorije();
            return View(proizvod);
        }

        // GET: Proizvodi/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proizvod proizvod = db.Proizvodi.Find(id);
            if (proizvod == null)
            {
                return HttpNotFound();
            }

            PopuniKategorije();

            //ViewBag.Kategorije = db.Kategorije.Select(x => new SelectListItem()
            //{
            //    Text = x.Naziv,
            //    Value = x.Id.ToString()
            //}
            //);

            return View(proizvod);
        }

        private void PopuniKategorije()
        {
            ViewBag.Kategorije = db.Kategorije.ToList();
        }

        // POST: Proizvodi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Naziv,Cena,Kategorija,Stanje")] Proizvod proizvod)
        {
            if (ModelState.IsValid)
            {
                var proizvodToUpdate = db.Proizvodi.Find(proizvod.Id);
                TryUpdateModel(proizvodToUpdate, new string[] { "Naziv", "Cena", "Stanje" });
                proizvodToUpdate.Kategorija = db.Kategorije.Find(proizvod.Kategorija.Id);

                //db.Entry(proizvod).State = EntityState.Modified;

                // snimimo u bazu
                db.SaveChanges();

                // redirekt na listu prozivoda
                return RedirectToAction("Index");
            }

            PopuniKategorije();

            return View(proizvod);
        }

        // GET: Proizvodi/Delete/5
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proizvod proizvod = db.Proizvodi.Find(id);
            if (proizvod == null)
            {
                return HttpNotFound();
            }
            return View(proizvod);
        }

        // POST: Proizvodi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult DeleteConfirmed(int id)
        {
            //Proizvod proizvod = db.Proizvodi.Find(id);
            //db.Proizvodi.Remove(proizvod);

            db.Proizvodi.RemoveRange(db.Proizvodi.Where(p => p.Id == id));
            

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Categories()
        {
            return View();
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
