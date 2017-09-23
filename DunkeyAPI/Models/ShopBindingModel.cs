using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class ShopBindingModel
    {

       
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }

        public string Description { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

    }

    public class ShopCategoriesBindingModel
    {

        [Required]
        public string Name { get; set; }

        //[Required]
        public string Description { get; set; }

        [Required]
        public short Store_Id { get; set; }

        public short Status { get; set; }

        public int ParentCategoryId { get; set; }
    }

}