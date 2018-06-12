using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop2018.Models;
using System.Linq.Dynamic;
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

            //privremeno resenje
            if (User.IsInRole(RolesConfig.USER))
            {
                var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                // trazimo otvorenu narudzbenicu
                var order = db.Orders.FirstOrDefault(o => o.State == OrderState.Open && o.User.Id == currentUser.Id);

                if (order != null && order.OrderLines.Count > 0)
                {
                    ViewBag.OrderLineCount = order.OrderLines.Count;
                    ViewBag.OrderPrice = order.OrderLines.Sum(ol => ol.Price * ol.Quantity);
                }
                else
                {
                    // TODO
                }
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



                //var image = proizvodIzForme.Images.FirstOrDefault(i => i.Proizvod.Id == proizvodIzForme.Id);

                //if (image == null)
                //{
                //    image = new Image()
                //    {
                //        Proizvod = proizvodIzForme,
                //        ImageName = proizvodIzForme.ImeSlike
                //    };

                //    db.Images.Add(image);
                //}

                // drugi save changes nam snima ime slike
                SnimiSlikuIDodeliImeSlikeProizvodu(proizvodIzForme, slika);

                db.SaveChanges();

                //SnimiSliku(image, slika);
                //db.SaveChanges();
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
            Proizvod proizvod = db.Proizvodi.Find(id);
            if (proizvod == null)
            {
                return HttpNotFound();
            }
            PostaviKategorije();
            return View(proizvod);
        }


        private void SnimiSlikuIDodeliImeSlikeProizvodu(Proizvod proizvod, HttpPostedFileBase slika)
        {
            if (slika != null)
            {
                proizvod.ImeSlike = slika.FileName;

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
            var item = db.Proizvodi.Find(id);
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

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

    //    private void SnimiSliku(Image image, HttpPostedFileBase slika)
    //    {
    //        if (slika != null)
    //        {
    //            image.ImageName = slika.FileName;

    //            var putanjaDoSlike = Server.MapPath($"~/Content/Artikli/{image.ImeSlikeZaPrikaz}");
    //            slika.SaveAs(putanjaDoSlike);

    //        }
    //    }


    //    [Authorize(Roles = RolesConfig.ADMIN)]
    //    public ActionResult AddImage(int id)
    //    {
    //        var item = db.Proizvodi.Find(id);
            
           
    //        var image = item.Images.FirstOrDefault(i => i.Proizvod.Id == item.Id);

    //        if (image == null)
    //        {
    //            image = new Image()
    //            {
    //                Proizvod = item,
    //                ImageName = item.ImeSlike
    //            };

    //            db.Images.Add(image);
    //        }

            
    //        //SnimiSliku(image, slika);
          
    //        db.SaveChanges();

    //        return RedirectToAction("Create");
    //    }
    }
}