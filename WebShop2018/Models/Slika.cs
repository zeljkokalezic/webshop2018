using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class Slika
    {
        public int Id { get; set; }
        public string Naziv { get; set; }

        public virtual Proizvod proizvod { get; set; }
    }
}