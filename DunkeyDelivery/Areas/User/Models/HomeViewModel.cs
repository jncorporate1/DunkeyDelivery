using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class HomeViewModel : AddressBar 
    {
      
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        [MinLength(10, ErrorMessage = "Phone Number is invalid ")]
        [MaxLength(15, ErrorMessage = "Phone Number is invalid")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

    }
    public class AddressBar : BaseViewModel
    {
        public string ErrorMessage { get; set; } = "";
        public string Address { get; set; }

    } 

    public class GiftViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Use Alphabets Only")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Payment Method required")]
        public bool PaymentMethod { get; set; }
    }
    public class ForgetPasswordEmail : BaseViewModel
    {
        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage ="Email Address is invalid")]
        public string Email { get; set; }
        
        public IEnumerable<ForgetPasswordToken> forgetPasswordToken { get; set; }
        public string Code { get; set; }
    }
    public class NewPassword : BaseViewModel
    {
  
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password must be at least 6 characters long", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Password must be at least 6 characters long", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password & Confirm Password donot match")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public ForgetPasswordToken forgetPasswordToken { get; set; }

        public string ErrorMessage { get; set; } = "";
    }
    public class SearchByName
    {
   
      public string BusinessName { get; set; }
    }
    public class ForgetPasswordToken
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public int User_Id { get; set; }
    }

}