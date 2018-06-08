using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;
using WebShop2018.Models;

namespace WebShop2018.Controllers
{
    public class KategorijeController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public KategorijeController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/Kategorije
        public IQueryable<Kategorija> GetKategorije()
        {
            //Thread.Sleep(10000);
            return db.Kategorije;
        }

        // GET: api/Kategorije/5
        [ResponseType(typeof(Kategorija))]
        public IHttpActionResult GetKategorija(int id)
        {
            Kategorija kategorija = db.Kategorije.Find(id);
            if (kategorija == null)
            {
                return NotFound();
            }

            return Ok(kategorija);
        }

        // PUT: api/Kategorije/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutKategorija(int id, Kategorija kategorija)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != kategorija.Id)
            {
                return BadRequest();
            }

            db.Entry(kategorija).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KategorijaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(kategorija);
            //return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Kategorije
        [ResponseType(typeof(Kategorija))]
        public IHttpActionResult PostKategorija(Kategorija kategorija)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Kategorije.Add(kategorija);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = kategorija.Id }, kategorija);
        }

        // DELETE: api/Kategorije/5
        [ResponseType(typeof(Kategorija))]
        public IHttpActionResult DeleteKategorija(int id)
        {
            Kategorija kategorija = db.Kategorije.Find(id);
            if (kategorija == null)
            {
                return NotFound();
            }

            db.Kategorije.Remove(kategorija);
            db.SaveChanges();

            return Ok(kategorija);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool KategorijaExists(int id)
        {
            return db.Kategorije.Count(e => e.Id == id) > 0;
        }
    }
}