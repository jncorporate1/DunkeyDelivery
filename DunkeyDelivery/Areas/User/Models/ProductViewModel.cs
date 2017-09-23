using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class ProductViewModel
    {

        public int Id { get; set; }

    
        public string Name { get; set; }

        public string Price { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public short Status { get; set; }

        public string Size { get; set; }

        public Store Store { get; set; }
    }
}