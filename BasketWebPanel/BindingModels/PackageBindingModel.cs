using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    public class PackageBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string NameNoUse  { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid price")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a valid price")]
        public double? Price { get; set; }

        public int Store_Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Description { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public List<Package_ProductBindingModel> Package_Products { get; set; }
    }

   

    public class Package_ProductBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string Qty { get; set; }

        public int Product_Id { get; set; }

        public int Package_Id { get; set; }
        
    }
}