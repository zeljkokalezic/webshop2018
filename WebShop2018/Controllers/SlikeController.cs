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

        // GET: Slike
        //[Authorize(Roles = RolesConfig.ADMIN)]
        //public ActionResult Index(SlikeGridViewModel viewModel)
        //{
        //    IQueryable<Slika> slike = db.Slike;
        //    if (viewModel.Query != null)
        //    {
        //        // daj sve proizvode gde ime sadrzi ovaj query parametar
        //        slike = slike.Where(s => s.NazivSlike.Contains(viewModel.Query));
        //    }

        //    if (viewModel.SortBy != null && viewModel.SortDirection != null)
        //    {
        //        // sortiranje koriscenjem Linq.Dynamic
        //        slike = slike.OrderBy(string.Format("{0} {1}", viewModel.SortBy, viewModel.SortDirection));
        //    }

        //    // paging
        //    slike = slike.Skip((viewModel.Page - 1) * viewModel.PageSize).Take(viewModel.PageSize);

        //    // vrati podatke iz baze :)
        //    viewModel.Slike = slike.ToList();

        //    return View(viewModel);
        //}

        // GET: Slike
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
            PostaviProizvode();
            return View();
        }

        // POST: Slike/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> slika, Proizvod proizvod)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                SnimiSliku(proizvod, slika);
                db.SaveChanges();

                return View("Index", "Artikli");
            }

            return RedirectToAction("Index");
        }

        public void SnimiSliku(Proizvod proizvod, IEnumerable<HttpPostedFileBase> slika)
        {
            if (slika != null)
            {
                foreach (var sl in slika)
                {
                    var slikaZaBazu = new Slika()
                    {
                        NazivSlike = Path.GetFileName(sl.FileName),
                        Proizvod = db.Proizvodi.Find(proizvod.Id)
                        //Proizvod=proizvod
                    };

                    //slikaZaBazu.Proizvod = db.Proizvodi.Find(slikaZaBazu.Proizvod.Id);

                    db.Slike.Add(slikaZaBazu);
                    db.SaveChanges();

                    var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{slikaZaBazu.NazivSlikeZaPrikaz}");
                    sl.SaveAs(putanjaDoSlike);
                }
            }
        }

        public void PostaviProizvode()
        {
            ViewBag.Proizvodi = db.Proizvodi.ToList();
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
        public ActionResult Edit([Bind(Include = "Id,NazivSlike,OpisSlike")] Slika slika)
        {
            if (ModelState.IsValid)
            {
                db.Entry(slika).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
        public ActionResult DeleteConfirmed(int id)
        {
            Slika slika = db.Slike.Find(id);
            int idProizvoda = slika.Proizvod.Id;
            db.Slike.Remove(slika);
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
