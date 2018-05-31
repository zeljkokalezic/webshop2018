using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public enum OrderState
    {
        Open,
        Closed,
        Shipped,
        Canceled,
        Processing
    }

    public class Order
    {
        // hocemo da nam baza sama napravi guid
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderState State { get; set; }

        public decimal Total
        {
            get
            {
                return OrderLines != null ? OrderLines.Sum(ol => ol.Price * ol.Quantity) : 0;
            }
        }

        public decimal TotalItems
        {
            get
            {
                return OrderLines != null ? OrderLines.Sum(ol => ol.Quantity) : 0;
            }
        }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}