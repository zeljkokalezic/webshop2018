using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop2018.Models;

namespace WebShop2018.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult PrimerZaView()
        {
            var proizvod = db.Proizvodi.First();//new Proizvod() { Naziv = "TestProizvod", Cena = 12345 };

            return View("Primer", proizvod);
        }

        //http://localhost:55710/Home/SkiniFajl?imeFajla=logo.png
        [Route("download/{imeFajla?}/")]
        public ActionResult SkiniFajl(string imeFajla)
        {
            if (imeFajla == null)
            {
                return HttpNotFound();
            }

            string putanjaDoFajla = Server.MapPath("~/Content/" + imeFajla);
            string tipFajla = MimeMapping.GetMimeMapping(imeFajla);

            return File(putanjaDoFajla, tipFajla, imeFajla);
        }

        public ActionResult Index()
        {
            int[] array = { 1, 2, 4, 5, 77, 99, 100, 1300 };

            var result = array.Where(i => Math.Asin(i) > 5);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact(string poruka)
        {
            ViewBag.Title = "Contact";
            ViewBag.Message = "Your contact page.";
            ViewBag.Poruka = new Proizvod { Id = -1, Cena = 999, Naziv = poruka };

            ViewData["Info"] = "Informacije neke tamo...";

            return View();
        }

        //[Route("freedownload/{year}/{category}", Order = 1)]
        public ActionResult RouteTest(int? year, string category)
        {
            return Content(string.Format("Year: {0} - Category: {1}", year, category));
        }

        public ActionResult FileResult(string imeFajla)
        {
            if (imeFajla == null)
            {
                return HttpNotFound();
            }

            var lokacija = Server.MapPath("~/Content/Images/" + imeFajla);
            return File(lokacija, MimeMapping.GetMimeMapping(imeFajla));
        }
    }
}