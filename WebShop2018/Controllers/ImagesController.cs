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
    public class ImagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Images
        public ActionResult Index()
        {
            return View(db.Images.ToList());
        }

        // GET: Images/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            PostaviProizvode();
            return View(image);
        }

        // GET: Images/Create
        public ActionResult Create()
        {
            PostaviProizvode();
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Image image, int proizvodId, HttpPostedFileBase slika)
        {
            if (ModelState.IsValid)
            {
                //image.Proizvod = db.Proizvodi.Find(image.Proizvod.Id);
                image.Proizvod = db.Proizvodi.Find(proizvodId);
                db.Images.Add(image);
                db.SaveChanges();
                SnimiSliku(image, slika);
                db.SaveChanges();

                return RedirectToAction("Index");


            }

            PostaviProizvode();
            return View(image);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,ImageName, Proizvod")] Image image, HttpPostedFileBase slika)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        image.Proizvod = db.Proizvodi.Find(image.Proizvod.Id);
        //        db.Images.Add(image);
        //        db.SaveChanges();
        //        SnimiSliku(image, slika);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    PostaviProizvode();
        //    return View(image);
        //}


        //public ActionResult AddImage(int? id)
        //{
        //    var item = db.Proizvodi.Find(id);


        //    var image = item.Images.FirstOrDefault(i => i.Proizvod.Id == item.Id);

        //    if (image == null)
        //    {
        //        image = new Image()
        //        {
        //            Proizvod = item,
        //            ImageName = item.ImeSlike
        //        };

        //        db.Images.Add(image);
        //    }


        //    //SnimiSliku(image, slika);

        //    db.SaveChanges();

        //    return RedirectToAction("Index");
        //}

        // GET: Images/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            PostaviProizvode();
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ImageName,Proizvod")] Image image)
        {
            if (ModelState.IsValid)
            {
                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PostaviProizvode();
            return View(image);
        }

        // GET: Images/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Image image = db.Images.Find(id);
            db.Images.Remove(image);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void PostaviProizvode()
        {
            ViewBag.Proizvodi = db.Proizvodi.ToList();
        }

        private void SnimiSliku(Image image, HttpPostedFileBase slika)
        {
            if (slika != null)
            {
                image.ImageName = slika.FileName;

                var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{image.ImeSlikeZaPrikaz}");
                slika.SaveAs(putanjaDoSlike);

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
