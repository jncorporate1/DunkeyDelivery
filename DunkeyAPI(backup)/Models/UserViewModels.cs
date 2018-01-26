using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PhoneVerificationViewModel
    {
        [Required]
        [Display(Name = "Request Id")]
        public string request_id { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }

    public class MessageViewModel
    {
        public string StatusCode { get; set; }
        public string Details { get; set; }
    }

    public class ImagePathViewModel
    {
        [Required]
        public string Path { get; set; }
    }
    public class PhoneBindingModel
    {
        [Required]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
    }
    public class ResetPassword
    {

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password must be at least 6 characters long", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Password must be at least 6 characters long", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password & Confirm Password donot match")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }


    }

    public class MobileViewModels
    {

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password must be at least 6 characters long", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Password must be at least 6 characters long", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password & Confirm Password donot match")]
        public string ConfirmPassword { get; set; }

        public int User_Id { get; set; }


    }


    public class PhoneVerificationModel
    {
        [Required]
        [Display(Name = "Request Id")]
        public string request_id { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}