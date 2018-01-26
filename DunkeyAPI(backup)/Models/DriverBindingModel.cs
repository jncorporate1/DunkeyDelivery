using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class DriverBindingModel
    {

        [DataType(DataType.Text)]
       
        [Display(Name = "Full Name")]
        public string FullName { get; set; }



        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }



        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }



        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        public Int16? Role { get; set; }
        
        public string HearFrom { get; set; }
    }
}