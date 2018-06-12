using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class Opis
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Opis proizvoda mora da se navede")]
        [StringLength(200)]
        [DisplayName("Opis Proizvoda")]
        public string OpisProizvoda { get; set; }

        public virtual ICollection<Proizvod> Proizvodi { get; set; }
    }
}