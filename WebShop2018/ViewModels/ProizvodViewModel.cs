using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop2018.Models;

namespace WebShop2018.ViewModels
{
    public class ProizvodViewModel
    {
        public int Id { get; set; }

        public StanjeProizvoda Stanje { get; set; }

        public string Naziv { get; set; }

        public decimal Cena { get; set; }

        public string Opis { get; set; }

        public virtual Kategorija Kategorija { get; set; }
        public virtual ICollection<Dobavljac> Dobavljaci { get; set; }
        public virtual ICollection<Slika> Slike { get; set; }

        public string SlikaZaPrikaz { get; set; }
        
    }
}