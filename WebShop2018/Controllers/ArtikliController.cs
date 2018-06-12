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
        public ActionResult Create(Proizvod proizvodIzForme, IEnumerable<HttpPostedFileBase> slika)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                //snimanje u bazu
                proizvodIzForme.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);
                db.Proizvodi.Add(proizvodIzForme);
                db.SaveChanges();

                //SnimiSlikuIDodeliImeSlikeProizvodu(proizvodIzForme, slike);
                SnimiSliku(proizvodIzForme, slika);

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

        //private void SnimiSlikuIDodeliImeSlikeProizvodu(Proizvod proizvod, IEnumerable<HttpPostedFileBase> slika)
        //{
        //    if (slika != null)
        //    {
        //        var listaSlika = slika.ToList();
        //        //stavljamo flag da nam ne bi u index view stalno ubacivao poslednju upload-ovanu sliku
        //        //na taj nacin pozivamo drugu funkciju u else grani
        //        var flag = true;
        //        foreach(var sl in listaSlika)
        //        {
        //            if (flag)
        //            {
        //                //proizvod.ImeSlike = Path.GetFileName(sl.FileName);
        //                //var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{proizvod.ImeSlikeZaPrikaz}");
        //                //sl.SaveAs(putanjaDoSlike);

        //                //var slikaZaBazu = new Slika()
        //                //{
        //                //    NazivSlike = proizvod.ImeSlikeZaPrikaz
        //                //};

        //                var proizvodIzBaze = db.Proizvodi.Find(proizvod.Id);

        //                //da ne bi mogle da se ubace slike koje vec postoje u bazi
        //                //neki uslov...
        //                //if (proizvod.Slike.FirstOrDefault(s => s.NazivSlike == s.Proizvod.ImeSlikeZaPrikaz) == null)
        //                //{
        //                //    slikaZaBazu.Proizvod = proizvod;
        //                //    db.Slike.Add(slikaZaBazu);

        //                //    flag = false;
        //                //}
        //                //slikaZaBazu.Proizvod = proizvodIzBaze;
        //                //db.Slike.Add(slikaZaBazu);

        //            flag = false;
        //        }
        //            else
        //            {
        //                //Pravimo drugu funkciju jer bez nje uvek se u listi proizvoda(index view) prikazuje
        //                //poslednja upoladovana slika, jer se stalno menja property ImeSlikeZaPrikaz.Umesto tog
        //                //property - ja u toj drugoj funkciji pozivamo funkciju ImeSlikeZaBazu
        //                SnimiSlikuUBazu(proizvod, slika);
        //            }

        //        }
        //    }
        //}

        //private string ImeSlikeZaBazu(Proizvod proizvod, string nazivSlike)
        //{
        //    return string.Empty;
        //    //return string.IsNullOrWhiteSpace(proizvod.ImeSlike) ? "no_image.png" : string.Format("{0}{1}", proizvod.Id, nazivSlike);
        //}

        //private void SnimiSlikuUBazu(Proizvod proizvod, IEnumerable<HttpPostedFileBase> slika)
        //{
        //    if (slika != null)
        //    {
        //        foreach (var sl in slika)
        //        {
        //            string nazivSlike = Path.GetFileName(sl.FileName);

        //            var slikaPostoji = proizvod.Slike.Any(s => s.NazivSlike == nazivSlike);

        //            if (!slikaPostoji)
        //            {

        //            }

        //            //var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{ImeSlikeZaBazu(proizvod, nazivSlike)}");
        //            //sl.SaveAs(putanjaDoSlike);

        //            //var slikaZaBazu = new Slika()
        //            //{
        //            //    NazivSlike = ImeSlikeZaBazu(proizvod, nazivSlike)
        //            //};

        //            ////if (proizvod.ImeSlikeZaPrikaz != slikaZaBazu.NazivSlike)
        //            ////{
        //            ////    proizvod = db.Proizvodi.Find(proizvod.Id);
        //            ////    slikaZaBazu.Proizvod = proizvod;
        //            ////    db.Slike.Add(slikaZaBazu);
        //            ////}
        //        }
        //    }
        //}

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
        public ActionResult Edit(Proizvod proizvodIzForme, IEnumerable<HttpPostedFileBase> slika)
        {
            if (ModelState.IsValid)
            {
                // nadji proizvod iz baze
                var proizvodIzBaze = db.Proizvodi.Find(proizvodIzForme.Id);
                // update vrednostima iz forme
                TryUpdateModel(proizvodIzBaze, new string[] { "Naziv", "Cena", "Stanje" });
                // dodela kategorije
                proizvodIzBaze.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);

                //SnimiSlikuIDodeliImeSlikeProizvodu(proizvodIzBaze, slika);
                SnimiSliku(proizvodIzBaze, slika);

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
    }
}