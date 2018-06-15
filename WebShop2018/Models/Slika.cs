using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class Slika
    {
        public int Id { get; set; }
        public string NazivSlike { get; set; }
        public string OpisSlike { get; set; }

        public string NazivSlikeZaPrikaz
        {
            get
            {
                return string.Format("{0}{1}", Proizvod.Id, NazivSlike);
            }
        }

        public virtual Proizvod Proizvod { get; set; }
    }
}