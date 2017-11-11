using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class SearchStoreCategoriesViewModel
    {
        public int Store_id { get; set; }
        public string BusinessType { get; set; }
        public double BusinessTypeTax { get; set; }
        public ShopViewModel shopViewModel { get; set; }
        public string SearchType { get; set; }
        public int Category_Id { get; set; }
        
    }
}