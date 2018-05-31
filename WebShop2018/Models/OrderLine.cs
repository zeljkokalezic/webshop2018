using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class OrderLine
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal Total
        {
            get
            {
                return Quantity * Price;
            }
        }


        public virtual Proizvod Item { get; set; }
        public virtual Order Order { get; set; }
    }
}