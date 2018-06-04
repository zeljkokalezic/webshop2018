using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageName { get; set; }

        public Proizvod Proizvod { get; set; }
    }
}