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
    [Authorize]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        [Authorize(Roles = RolesConfig.ADMIN + "," + RolesConfig.USER)]
        public ActionResult Index()
        {
            IEnumerable<Order> orders = null;

            // ako je neko u roli korisnik prikazi samo njegove narudzbine
            if (User.IsInRole(RolesConfig.USER))
            {
                orders = db.Orders.Where(o => o.User.UserName == User.Identity.Name);
            }
            // ako je administrator neka se prikazu sve
            else if (User.IsInRole(RolesConfig.ADMIN))
            {
                orders = db.Orders;
            }

            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        [Authorize(Roles = RolesConfig.ADMIN + "," + RolesConfig.USER)]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole(RolesConfig.USER))
            {
                // ako trenutno ulogovani korisnik ima order u svojoj listi ordera
                //var currentUser = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
                //var canViewOrder = currentUser.Orders.Any(o => o.Id == order.Id);

                // ako je username trenutno ulogovanog korisnika jednak username-u korsnika cija je narudzbina
                var viewForbbiden = order.User.UserName != User.Identity.Name;

                if (viewForbbiden)
                {
                    return HttpNotFound();
                }
            }

            return View(order);
        }

        // GET: Orders/Details/5
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult AdminDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        [Authorize(Roles = RolesConfig.USER)]
        public ActionResult UserDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var forbbidenView = order.User.UserName != User.Identity.Name;
            if (forbbidenView)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Create([Bind(Include = "Id,Comment,CreatedAt,State")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Id = Guid.NewGuid();
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Edit([Bind(Include = "Id,Comment,CreatedAt,State")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = RolesConfig.USER)]
        [HttpPost]
        public ActionResult FinalizeOrder(Guid id, string comment)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var forbbidenView = order.User.UserName != User.Identity.Name;
            if (forbbidenView)
            {
                return HttpNotFound();
            }

            order.Comment = comment;
            order.State = OrderState.Processing;

            db.SaveChanges();

            return View("ThankYou");
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
