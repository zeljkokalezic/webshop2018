using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebShop2018.Models
{
    public class SlikeGridViewModel : BaseGridViewModel
    {
        // za pretragu
        public string Query { get; set; }

        // za sortiranje
        public string SortBy { get; set; } = "NazivSlike";
        public string SortDirection { get; set; } = "ASC";

        // lista proizvoda za grid
        public List<Slika> Slike { get; set; }

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
                Query,
                PageSize,
                page
            };
        }
    }
}