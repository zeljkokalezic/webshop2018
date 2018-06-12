using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public class ArtikliGridViewModel
    {
        // za pretragu
        public string Query { get; set; }
        public int? MaxPrice { get; set; }

        // za sortiranje
        public string SortBy { get; set; } = "Naziv";
        public string SortDirection { get; set; } = "ASC";


        public Order Order { get; set; }


        // za paginaciju
        public int PageSize { get; set; } = 5;
        public int Page { get; set; } = 1;
        public int Count { get; set; }
        public int TotalPages
        {
            get { return (Count + PageSize - 1) / PageSize; }
        }

        // lista proizvoda za grid
        public List<Proizvod> Artikli { get; set; }

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
                MaxPrice,
                Query,
                PageSize,
                Page
            };
        }

        public object GetPagingParameters(int page)
        {
            return new
            {
                SortBy,
                SortDirection,
                MaxPrice,
                Query,
                PageSize,
                page
            };
        }
    }
}