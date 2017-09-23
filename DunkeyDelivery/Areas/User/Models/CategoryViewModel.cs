using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class CategoryViewModel 
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentCategoryId { get; set; }
        public string Type { get; set; }
        //public string Description { get; set; }
        
    }
}