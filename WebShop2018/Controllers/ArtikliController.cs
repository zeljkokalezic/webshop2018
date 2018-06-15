using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop2018.Models;
using System.Linq.Dynamic;
using System.IO;
using System.Net;

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

        [AllowAnonymous]
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
        public ActionResult Create(Proizvod proizvodIzForme, IEnumerable<HttpPostedFileBase> listaSlika)
        {
            if (ModelState.IsValid)
            {
                //snimanje u bazu
                proizvodIzForme.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);
                db.Proizvodi.Add(proizvodIzForme);
                db.SaveChanges();

                SnimiSliku(proizvodIzForme, listaSlika);

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

        public void SnimiSliku(Proizvod proizvod, IEnumerable<HttpPostedFileBase> listaSlika)
        {
            if (listaSlika != null)
            {
                foreach (var sl in listaSlika)
                {
                    if (sl != null)
                    {
                        var slikaZaBazu = new Slika()
                        {
                            NazivSlike = Path.GetFileName(sl.FileName),
                            Proizvod = db.Proizvodi.Find(proizvod.Id)
                            //Proizvod = proizvod
                        };

                        db.Slike.Add(slikaZaBazu);
                        db.SaveChanges();

                        var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{slikaZaBazu.NazivSlikeZaPrikaz}");
                        sl.SaveAs(putanjaDoSlike);
                    }
                }
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
            return View("Edit", proizvodIzBaze);
        }

        [HttpPost]
        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Edit(Proizvod proizvodIzForme, IEnumerable<HttpPostedFileBase> listaSlika)
        {
            if (ModelState.IsValid)
            {
                // nadji proizvod iz baze
                var proizvodIzBaze = db.Proizvodi.Find(proizvodIzForme.Id);
                // update vrednostima iz forme
                TryUpdateModel(proizvodIzBaze, new string[] { "Naziv", "Cena", "Stanje" });
                // dodela kategorije
                proizvodIzBaze.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);

                SnimiSliku(proizvodIzBaze, listaSlika);

                // snimi u bazu
                db.SaveChanges();

                // vrati na listu ako je sve u redu
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Edit";
            PostaviKategorije();
            return View("Edit", proizvodIzForme);
        }

        [Authorize(Roles = RolesConfig.ADMIN)]
        public ActionResult Delete(int id)
        {
            var proizvodIzBaze = db.Proizvodi.Find(id);
            //moramo prvo da izbrisemo slike koje su vezane za proizvod koji brisemo, jer se u tabeli
            //Slike nalazi strani kljuc ProizvodId
            var slikeProizvoda = proizvodIzBaze.Slike.ToList();
            foreach(var slika in slikeProizvoda)
            {
                db.Slike.Remove(slika);
            }
            db.Proizvodi.Remove(proizvodIzBaze);
            db.SaveChanges();

            return RedirectToAction("Index");
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

        public ActionResult DodajOpis(int id, string opisSlike)
        {
            var slika = db.Slike.Find(id);

            slika.OpisSlike = opisSlike;
            db.SaveChanges();

            var proizvodId = slika.Proizvod.Id;

            return RedirectToAction("Edit", new { id = proizvodId });
        }

        public ActionResult ObrisiSliku(int id)
        {
            Slika slika = db.Slike.Find(id);

            //brisemo najpre iz Content foldera
            var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{slika.NazivSlikeZaPrikaz}");

            if (System.IO.File.Exists(putanjaDoSlike))
            {
                System.IO.File.Delete(putanjaDoSlike);
            }

            var proizvodId = slika.Proizvod.Id;

            db.Slike.Remove(slika);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = proizvodId });
        }
    }
}