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
        public ActionResult Create(Proizvod proizvodIzForme, IEnumerable<HttpPostedFileBase>slika,string e)
        {
            if (ModelState.IsValid)
            {
                //snimanje u bazu
                proizvodIzForme.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);
                db.Proizvodi.Add(proizvodIzForme);
                db.SaveChanges();

                UploadPicture(slika,proizvodIzForme ,e);

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

        public void UploadPicture(IEnumerable<HttpPostedFileBase> slika, Proizvod modelToUpdate, string opis)
        {

            if (slika != null)
            {
                foreach (var file in slika)
                {
                    if (file.ContentLength > 0)
                    {
                        var postojecaSlika = db.Slike.FirstOrDefault(o => o.Naziv == file.FileName && o.proizvod.Id == modelToUpdate.Id);
                        if (postojecaSlika == null)
                        {
                            var sl = new Slike()
                            {

                                Naziv = file.FileName,
                                proizvod = modelToUpdate,
                                Opis = opis

                            };
                            db.Slike.Add(sl);

                            var putanjaDoFoldera = Server.MapPath(string.Format("~/Content/vezbaSlike/{0}{1}", modelToUpdate.Id, file.FileName));
                            //var putanjaDoFoldera = Server.MapPath(string.Format($"~/Content/vezbaSlike/{modelToUpdate.NoImage}"));
                            file.SaveAs(putanjaDoFoldera);
                        }



                    }
                }
            }
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
            return View(proizvod);
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
        public ActionResult Edit(Proizvod proizvodIzForme, IEnumerable<HttpPostedFileBase> slika,string e)
        {
            if (ModelState.IsValid)
            {
                // nadji proizvod iz baze
                var proizvodIzBaze = db.Proizvodi.Find(proizvodIzForme.Id);
                // update vrednostima iz forme
                TryUpdateModel(proizvodIzBaze, new string[] { "Naziv", "Cena", "Stanje" });
                // dodela kategorije
                proizvodIzBaze.Kategorija = db.Kategorije.Find(proizvodIzForme.Kategorija.Id);

                UploadPicture(slika,proizvodIzForme,e);

                // snimi u bazu
                db.SaveChanges();

                // vrati na listu ako je sve u redu
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Edit";
            PostaviKategorije();
            return View("Create", proizvodIzForme);
        }


        public ActionResult DeletePicture(int id)
        {
            // uzmemo sliku koju brisemo
            var slikaZaBrisanje = db.Slike.Find(id);
            var proizvodId = slikaZaBrisanje.proizvod.Id;

            // brisemo iz fajl sistema
            var fileName = string.Format("{0}{1}", proizvodId, slikaZaBrisanje.Naziv);
            var filePath = Server.MapPath($"~/Content/vezbaSlike/{fileName}");
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // brisemo iz baze
            db.Slike.Remove(slikaZaBrisanje);
            db.SaveChanges();
 

            return RedirectToAction("Details", new { id = proizvodId });
        }
        //get
        public ActionResult AddPicture(int id)
        {
            var pro = db.Proizvodi.Find(id);

            if (pro == null)
            {
                return HttpNotFound();
            }

            return View("AddPicture");
        }
        [HttpPost]
        public ActionResult AddPicture(int id, IEnumerable<HttpPostedFileBase> slika, string opis)
        {
            ViewBag.Text = "AddPicture";
            var modelToUpdate = db.Proizvodi.Find(id);

            if (ModelState.IsValid)
            {


                UploadPicture(slika, modelToUpdate, opis);
                db.SaveChanges();

            }

            return RedirectToAction("Details", new { id = modelToUpdate.Id });
        }
        //get
        public ActionResult EditPicture(int id, string ime)
        {
            var slika = db.Slike.Find(id);
            var idProizvoda = slika.proizvod.Id;
            var proizvod = db.Proizvodi.Find(idProizvoda);


            if (proizvod == null)
            {
                return HttpNotFound();
            }

            return View("EditPicture");
        }
        [HttpPost]
        public ActionResult EditPicture(int id, IEnumerable<HttpPostedFileBase> slika, string opis)
        {

            //slika za brisanje
            var slikaZaDelete = db.Slike.Find(id);
            var idProizvoda = slikaZaDelete.proizvod.Id;
            var proizvod = db.Proizvodi.Find(idProizvoda);

            ViewBag.Text = "EditPicture";

            if (ModelState.IsValid)
            {

                UploadPicture(slika, proizvod, opis);

                var fileName = string.Format("{0}{1}", idProizvoda, slikaZaDelete.Naziv);
                var filePath = Server.MapPath($"~/Content/vezbaSlike/{fileName}");
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                db.Slike.Remove(slikaZaDelete);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = idProizvoda });
            }

            return View();
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