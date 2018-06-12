using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class Slika
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }

        public virtual Proizvod Proizvod { get; set; }
    }
}