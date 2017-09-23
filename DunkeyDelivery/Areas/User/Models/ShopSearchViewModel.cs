using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class ShopSearchViewModel
    {
        public IEnumerable<ShopViewModel> Shops { get; set; }
        public bool IsValidAddress { get; set; }
    }
}