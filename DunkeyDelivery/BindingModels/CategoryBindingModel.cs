using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.BindingModels
{
    public class CategoryBindingModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}