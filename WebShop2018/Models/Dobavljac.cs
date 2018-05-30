using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebShop2018.Models
{
    public class Dobavljac
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public bool Aktivan { get; set; }

        public virtual ICollection<Proizvod> Proizvodi { get; set; }
    }

    public class OnlineDobavljac : Dobavljac
    {
        [EmailAddress]
        public string Email { get; set; }
        [Url]
        public string WebsiteURL { get; set; }
    }
}