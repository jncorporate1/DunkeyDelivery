using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class CareerViewModel
    {
        


    }
    public class CareerApplyViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

       
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        [StringLength(10, ErrorMessage = "Phone Number Should Be Of Max Length 10")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Cover Letter")]
        public string CoverLetter { get; set; }

        [Required]
        public string FileName { get; set; }

    }
}