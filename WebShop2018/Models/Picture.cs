using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string ImePrikazaneSlike { get { return string.Format("{0}{1}", Proizvod.Id, Naziv); } }


        public virtual Proizvod Proizvod { get; set; }
    }
}