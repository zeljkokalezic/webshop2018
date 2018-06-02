using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class OrdersGridViewModel : BaseGridViewModel
    {
        // za pretragu
        public string Query { get; set; }
        public int? MaxTotal { get; set; }

        // za sortiranje
        public string SortBy { get; set; } = "User.Username";
        public string SortDirection { get; set; } = "ASC";

        // lista proizvoda za grid
        public List<Order> Orders { get; set; }

        public object GetSortingParameters(string sortBy)
        {
            // default smer sortiranja
            var direction = "ASC";

            // menjamo smer sortiranja ako je vec sortirano po istom parametru
            if (SortBy == sortBy)
            {
                // ako je bio ASC bice DESC i obrnuto
                direction = SortDirection == "ASC" ? "DESC" : "ASC";
            }

            return new
            {
                sortBy,
                SortDirection = direction,
                MaxTotal,
                Query,
                PageSize,
                Page
            };
        }

        public override object GetPagingParameters(int page)
        {
            return new
            {
                SortBy,
                SortDirection,
                MaxTotal,
                Query,
                PageSize,
                page
            };
        }
    }
}