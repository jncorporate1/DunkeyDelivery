using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class ProductBindingModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Product Price")]
        public string Price { get; set; }

      
        public int Category_Id { get; set; }


        public int Store_Id { get; set; }

       
        public string ImageUrl { get; set; }

        [Required]
        public short Status { get; set; }

        public string Description { get; set; }
    }
}