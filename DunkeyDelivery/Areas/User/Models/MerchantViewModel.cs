using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class MerchantViewModel
    {
    }
    public class RegisterMerchantViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Without Spaces")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only Without Spaces")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Business Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Use Alphabets Only")]
        [Display(Name = "BusinessName")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Business Type Required")]
        public string BusinessType { get; set; }




        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }



        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        [MinLength(10, ErrorMessage = "Phone Number is invalid ")]
        [MaxLength(15, ErrorMessage = "Phone Number is invalid")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Zip Code is required")]
        [Display(Name = "Zip Code")]
        [RegularExpression(@"^\d{5}-\d{4}|\d{5}|[A-Z]\d[A - Z] \d[A - Z]\d$", ErrorMessage = "Enter a valid Zip Code")]
        [DataType(DataType.Text)]
        public string ZipCode { get; set; }


        public Int16 Role { get; set; }

        public short? Status { get; set; }

    }


}