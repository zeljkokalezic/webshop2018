using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop2018.Models;
using System.Linq.Dynamic;
using System.IO;

namespace WebShop2018.Controllers
{
    [Authorize]
    public class ArtikliController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Artikli
        [AllowAnonymous]
        public ActionResult Index(ArtikliGridViewModel viewModel)
        {
            IQueryable<Proizvod> proizvodi = db.Proizvodi;
            if (viewModel.Query != null)
            {
                // daj sve proizvode gde ime sadrzi ovaj query parametar
                proizvodi = proizvodi.Where(p => p.Naziv.Contains(viewModel.Query));
            }
            if (viewModel.MaxPrice.HasValue)
            {
                // svi proizvodi koji imaju cenu manju od
                proizvodi = proizvodi.Where(p => p.Cena <= viewModel.MaxPrice);
            }
            if (viewModel.SortBy != null && viewModel.SortDirection != null)
            {
                // sortiranje koriscenjem Linq.Dynamic
                proizvodi = proizvodi.OrderBy(string.Format("{0} {1}", viewModel.SortBy, viewModel.SortDirection));
            }

            // uzmemo ukupan broj proizvoda iz baze koji zaovoljavaju kriterijume pretrage
            viewModel.Count = proizvodi.Count();

            // paging
            proizvodi = proizvodi.Skip((viewModel.Page - 1) * viewModel.PageSize).Take(viewModel.PageSize);

            // vrati podatke iz baze :)
            viewModel.Artikli = proizvodi.ToList();

            if (User.IsInRole(RolesConfig.USER))
            {
                viewModel.Order = db.Orders.FirstOrDefault(o => o.State == OrderState.Open &&
                    o.User.UserName == User.Identity.Name);
            }


            // vracamo view sa view modelom
            return View(viewModel);
        }

        // GET
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Create()
        {
            @ViewBag.Title = "Create";
            PostaviKategorije();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Create(Proizvod proizvodIzForme, HttpPostedFileBase slika)
        {
            if (ModelState.IsValid)
            {
                //snimanje u bazu
                proizvodIzForme.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);
                db.Proizvodi.Add(proizvodIzForme);
                db.SaveChanges();

                SnimiSlikuIDodeliImeSlikeProizvodu(proizvodIzForme, slika);

                // drugi save changes nam snima ime slike
                db.SaveChanges();

                //ako je sve uredu salje na index akciju (lista proizvoda)
                return RedirectToAction("Index");
            }

            // vraca formu sa podacima u slucaju greske
            ViewBag.Title = "Create";
            PostaviKategorije();
            return View(proizvodIzForme);
        }

        private void SnimiSlikuIDodeliImeSlikeProizvodu(Proizvod proizvod, HttpPostedFileBase slika)
        {
            if (slika != null)
            {
                proizvod.ImeSlike = Path.GetFileName(slika.FileName);

                var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{proizvod.ImeSlikeZaPrikaz}");
                slika.SaveAs(putanjaDoSlike);
                
            }
        }

        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Edit(int id)
        {
            var proizvodIzBaze = db.Proizvodi.Find(id);

            //vraca 404 ako proizvod nije nadjen
            if (proizvodIzBaze == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Edit";
            PostaviKategorije();
            return View("Create", proizvodIzBaze);
        }

        [HttpPost]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Edit(Proizvod proizvodIzForme, HttpPostedFileBase slika)
        {
            if (ModelState.IsValid)
            {
                // nadji proizvod iz baze
                var proizvodIzBaze = db.Proizvodi.Find(proizvodIzForme.Id);
                // update vrednostima iz forme
                TryUpdateModel(proizvodIzBaze, new string[] { "Naziv", "Cena", "Stanje" });
                // dodela kategorije
                proizvodIzBaze.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);

                SnimiSlikuIDodeliImeSlikeProizvodu(proizvodIzBaze, slika);

                // snimi u bazu
                db.SaveChanges();

                // vrati na listu ako je sve u redu
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Edit";
            PostaviKategorije();
            return View("Create", proizvodIzForme);
        }

        [Authorize(Roles = RolesConfig.ADMIN)]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var proizvodIzBaze = db.Proizvodi.Find(id);

            //var orderLinesForDelete = db.OrderLines.Where(ol => ol.Item.Id == proizvodIzBaze.Id);
            //db.OrderLines.RemoveRange(orderLinesForDelete);

            db.Proizvodi.Remove(proizvodIzBaze);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                // logujemo exception ili tako nesto
                return new HttpStatusCodeResult(500);
            }

            return new HttpStatusCodeResult(200);
        }

        public void PostaviKategorije()
        {
            ViewBag.Kategorije = db.Kategorije.ToList();
        }

        [Authorize(Roles = RolesConfig.USER)]
        public ActionResult AddToCart(int id)
        {
            var currentUser = db.Users.First(u => u.UserName == User.Identity.Name);
            var item = db.Proizvodi.Find(id);
            
            // trazimo otvorenu narudzbenicu
            var order = db.Orders.FirstOrDefault(o => o.State == OrderState.Open && o.User.Id == currentUser.Id);

            // ako je ne pronadjemo treba da je napravimo
            if (order == null)
            {
                order = new Order()
                {
                    User = currentUser,
                    CreatedAt = DateTime.Now,
                    State = OrderState.Open,
                    OrderLines = new List<OrderLine>()
                };

                db.Orders.Add(order);
            }

            // dodamo order line ako ne postoji ili samo povecamo quantity
            var orderLine = order.OrderLines.FirstOrDefault(ol => ol.Item.Id == item.Id);

            if (orderLine == null)
            {
                orderLine = new OrderLine()
                {
                    Order = order,
                    Item = item,
                    Price = item.Cena
                };

                db.OrderLines.Add(orderLine);
            }

            orderLine.Quantity++;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}