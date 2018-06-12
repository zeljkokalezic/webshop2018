using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop2018.Models;

namespace WebShop2018.ViewModels
{
    public class ArtikliGridViewModel : BaseGridViewModel
    {
        // za pretragu
        public string Query { get; set; }
        public int? MaxPrice { get; set; }

        // za sortiranje
        public string SortBy { get; set; } = "Naziv";
        public string SortDirection { get; set; } = "ASC";

        // lista proizvoda za grid
        public List<ProizvodViewModel> Artikli { get; set; }

        // otvorena narudzbenica za trenutnog korisnika
        public Order Order { get; set; }

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

        public override object GetPagingParameters(int page)
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