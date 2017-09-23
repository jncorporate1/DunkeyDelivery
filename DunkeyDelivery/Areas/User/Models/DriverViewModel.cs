using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class DriverViewModel
    {
        
    }
    public class RegisterDriverViewModel : BaseViewModel
    {
        [Required(ErrorMessage ="Full Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Use Alphabets Only")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }



        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Email Address is invalid")]
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



        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Vehicle Type is required")]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }
    }
}