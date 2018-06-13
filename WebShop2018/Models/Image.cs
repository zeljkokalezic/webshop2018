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
        public string Description { get; set; }

        public Proizvod Proizvod { get; set; }


        public string ImeSlikeZaPrikaz
        {
            get
            {
                // nije bas najpametnije resenje
                return string.IsNullOrWhiteSpace(ImageName) ? "no_image.png" : string.Format("{0}{1}",Id, ImageName);
            }
        }
    }
}