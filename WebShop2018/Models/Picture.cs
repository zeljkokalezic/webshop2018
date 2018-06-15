using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class Picture
    {
        public int Id { get; set; }

        public string NazivSlike { get; set; }

        public string NazivSlikeZaPrikaz
        {
            get
            {               
                return string.IsNullOrWhiteSpace(NazivSlike) ? "no_image.png" : string.Format("{0}{1}", Id, NazivSlike);
            }
        }

        public string Opis { get; set; }

        public virtual Proizvod Proizvod { get; set; }
    }
}