using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebShop2018.Controllers
{
    public class Item
    {
        public List<Colour> AvailableColours { get; set; }
    }

    public class Colour
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public bool Checked { get; set; }

    }

    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            // test data
            var item = new Item()
            {
                AvailableColours = new List<Colour>()
            };

            for (int i = 0; i < 10; i++)
            {
                var colour = new Colour()
                {
                    ID = i,
                    Description = i.ToString(),
                    Checked = i % 2 == 0
                };
                item.AvailableColours.Add(colour);
            }

            return View("Test", item);
        }

        public ActionResult Create(Item model)
        {
            ViewBag.Test = "Created";
            return View("Test", model);
        }
    }
}