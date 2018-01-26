using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class MerchantBindingModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }


        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Role")]
        public Int16 Role { get; set; }



    }
}