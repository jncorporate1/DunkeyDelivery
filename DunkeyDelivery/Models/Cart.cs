using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Models
{
    public class Cart
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public int Price { get; set; }
        public int Type { get; set; }
    }
}