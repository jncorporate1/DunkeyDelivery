using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.Dashboard.Models
{
    public class CategoryProductViewModel : BaseViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Product Price")]
        public string Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Category.")]
        public int Category_id { get; set; }

        [Required]
        public int Store_id { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public short Status { get; set; }

        public string Description { get; set; }


        public SelectList StoreOptions { get; set; }

        public SelectList CategoryOptions { get; internal set; }


    }
}