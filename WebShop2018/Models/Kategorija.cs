using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class Kategorija
    {
        public int Id { get; set; }
        [Display(Name = "Naziv Kategorije:")]
        public string Naziv { get; set; }

        public virtual ICollection<Proizvod> Proizvodi { get; set; }
    }
}