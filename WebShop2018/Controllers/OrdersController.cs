using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebShop2018.Models;
using System.Linq.Dynamic;

namespace WebShop2018.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        [Authorize(Roles = RolesConfig.ADMIN + "," + RolesConfig.USER)]
        public ActionResult Index(OrdersGridViewModel viewModel)
        {
            //RouteValueDictionary test = new RouteValueDictionary(new { Pera = "Detlic"});
            //ViewBag.Test = new RouteValueDictionary(test.Union(new RouteValueDictionary(new { Mile = "Kitic" })).ToDictionary(k => k.Key, k => k.Value));

            IQueryable<Order> orders = null;

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

            if (viewModel.Query != null)
            {
                // daj sve ordere gde korisnicko ime sadrzi ovaj query parametar
                orders = orders.Where(o => o.User.UserName.Contains(viewModel.Query));
            }
            if (viewModel.MaxTotal.HasValue)
            {
                // svi proizvodi koji imaju cenu manju od
                orders = orders.Where(o => o.OrderLines.Sum(ol => ol.Quantity * ol.Price) <= viewModel.MaxTotal);
            }
            if (viewModel.SortBy != null && viewModel.SortDirection != null)
            {
                // sortiranje koriscenjem Linq.Dynamic
                if (viewModel.SortBy.ToLower().Equals("total"))
                {
                    if (viewModel.SortDirection.ToLower().Equals("asc"))
                    {
                        orders = orders.OrderBy(o => o.OrderLines.Sum(ol => ol.Quantity * ol.Price));
                    }
                    else
                    {
                        orders = orders.OrderByDescending(o => o.OrderLines.Sum(ol => ol.Quantity * ol.Price));
                    }
                }
                else
                {
                    orders = orders.OrderBy(string.Format("{0} {1}", viewModel.SortBy, viewModel.SortDirection));
                }
                
            }

            // uzmemo ukupan broj proizvoda iz baze koji zaovoljavaju kriterijume pretrage
            viewModel.Count = orders.Count();

            // paging
            orders = orders.Skip((viewModel.Page - 1) * viewModel.PageSize).Take(viewModel.PageSize);

            // vrati podatke iz baze :)
            viewModel.Orders = orders.ToList();


            return View(viewModel);
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

            if (order == null || OrderAcessNotAuthorized(order))
            {
                return HttpNotFound();
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
            if (order == null || OrderAcessNotAuthorized(order) || order.State != OrderState.Open)
            {
                return HttpNotFound();
            }

            order.Comment = comment;
            order.State = OrderState.Processing;

            db.SaveChanges();

            return View("ThankYou");
        }

        public ActionResult DeleteOrderLine(int id)
        {
            var orderLine = db.OrderLines.Find(id);

            if (orderLine == null 
                || orderLine.Order == null
                || orderLine.Order.State != OrderState.Open
                || OrderAcessNotAuthorized(orderLine.Order))
            {
                return HttpNotFound();
            }

            var orderId = orderLine.Order.Id;

            db.OrderLines.Remove(orderLine);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = orderId });
        }

        public ActionResult ChangeQuantity(int id, int quantity)
        {
            var orderLine = db.OrderLines.Find(id);

            if (orderLine == null
                || orderLine.Order == null
                || orderLine.Order.State != OrderState.Open
                || OrderAcessNotAuthorized(orderLine.Order))
            {
                return HttpNotFound();
            }

            var orderId = orderLine.Order.Id;

            orderLine.Quantity = quantity;

            // ovde ne dozvoljavamo da se kolicina smanji ispod 1
            if (orderLine.Quantity >= 1)
            {
                db.SaveChanges();
            }

            //// ovo je logika kada brisemo ako ima manje od 1
            //if (orderLine.Quantity <= 0)
            //{
            //    //// nacin brisanje na licu mesta
            //    //db.OrderLines.Remove(orderLine);
            //    //db.SaveChanges();

            //    // nacin redirect na delete akciju
            //    return RedirectToAction("DeleteOrderLine", new { id = orderLine.Id });
            //}

            return RedirectToAction("Details", new { id = orderId });
        }

        private bool OrderAcessNotAuthorized(Order order)
        {
            return User.IsInRole(RolesConfig.ADMIN) ? false : order.User.UserName != User.Identity.Name;
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
