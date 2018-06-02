using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop2018.Models
{
    public abstract class BaseGridViewModel
    {
        // za paginaciju
        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 1;
        public int Count { get; set; }
        public int TotalPages
        {
            get { return (Count + PageSize - 1) / PageSize; }
        }

        public abstract object GetPagingParameters(int page);
    }
}