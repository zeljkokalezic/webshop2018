using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop2018.Models;
using System.Linq.Dynamic;
using System.IO;
using System.Net;
using WebShop2018.ViewModels;

namespace WebShop2018.Controllers
{
    [Authorize]
    public class ProizvodiController : Controller
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

            var rezultat = proizvodi.ToList();

            // vrati podatke iz baze :)
            viewModel.Artikli = rezultat.Select(dbProizvod => 
            {
                return new ProizvodViewModel
                {
                    Id = dbProizvod.Id,
                    Cena = dbProizvod.Cena,
                    Dobavljaci = dbProizvod.Dobavljaci,
                    Kategorija = dbProizvod.Kategorija,
                    Naziv = dbProizvod.Naziv,
                    Stanje = dbProizvod.Stanje,
                    Slike = dbProizvod.Slike,
                    SlikaZaPrikaz = dbProizvod.Slike.FirstOrDefault(slika => slika.Id == dbProizvod.SlikaZaPrikazId)?.Naziv,
                    Opis = dbProizvod.Opis
                };
            }).ToList();

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
        public ActionResult Create(Proizvod proizvodIzForme, IEnumerable<HttpPostedFileBase> slika)
        {
            if (ModelState.IsValid)
            {
                //snimanje u bazu
                proizvodIzForme.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);
                db.Proizvodi.Add(proizvodIzForme);
                db.SaveChanges();

                var dbSlike = SnimiSlikuIDodeliImeSlikeProizvodu(proizvodIzForme, slika);

                db.Slike.AddRange(dbSlike);

                // drugi save changes nam snima ime slike
                db.SaveChanges();

                proizvodIzForme.SlikaZaPrikazId = dbSlike.FirstOrDefault()?.Id;

                db.SaveChanges();

                //ako je sve uredu salje na index akciju (lista proizvoda)
                return RedirectToAction("Index");
            }

            // vraca formu sa podacima u slucaju greske
            ViewBag.Title = "Create";
            PostaviKategorije();
            return View(proizvodIzForme);
        }


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

        public ActionResult DeleteImage (int? id, string imeSlike)
        {
            var proizvod = db.Proizvodi.Find(id);

            DirectoryInfo brisiSliku = new DirectoryInfo(@"C:\Users\Milica\Desktop\webshop2018-master\WebShop2018\Content\Artikli");

            string imeFajla = string.Format("{0}{1}", proizvod.Id, imeSlike);

            foreach (FileInfo item in brisiSliku.GetFiles())
            {
                if (item.Name == imeFajla)
                {
                    item.Delete();
                }
            }

            var redZaBrisanje = db.Slike.Where(sl => sl.Proizvod.Id == proizvod.Id && sl.Naziv == imeSlike);
            db.Slike.RemoveRange(redZaBrisanje);

            db.SaveChanges();

            return RedirectToAction("Details",new {id = proizvod.Id });


        }



        public IEnumerable<Slika> SnimiSlikuIDodeliImeSlikeProizvodu(Proizvod proizvod, IEnumerable<HttpPostedFileBase> slike)
        {
            if (slike == null)
                return null;

            var dbSlike = new List<Slika>();

            foreach (var slika in slike)
            {
                if (slika == null)
                    continue;

                var novaSlika = new Slika
                {
                    Naziv = $"{proizvod.Id}_{slika.FileName}",
                    Proizvod = proizvod
                    
                };

                dbSlike.Add(novaSlika);

                var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{novaSlika.Naziv}");
                slika.SaveAs(putanjaDoSlike);
            }

            return dbSlike;
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
        public ActionResult Edit(Proizvod proizvodIzForme, IEnumerable<HttpPostedFileBase> slika)
        {
            if (ModelState.IsValid)
            {
                // nadji proizvod iz baze
                var proizvodIzBaze = db.Proizvodi.Find(proizvodIzForme.Id);
                // update vrednostima iz forme
                TryUpdateModel(proizvodIzBaze, new string[] { nameof(Proizvod.Naziv), nameof(Proizvod.Cena), nameof(Proizvod.Stanje), nameof(Proizvod.SlikaZaPrikazId) });
                // dodela kategorije
                proizvodIzBaze.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);

                var dbSlike = SnimiSlikuIDodeliImeSlikeProizvodu(proizvodIzBaze, slika);

                db.Slike.AddRange(dbSlike);

                
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
        public ActionResult Delete(int id)
        {
            var proizvodIzBaze = db.Proizvodi.Find(id);
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
    }
}