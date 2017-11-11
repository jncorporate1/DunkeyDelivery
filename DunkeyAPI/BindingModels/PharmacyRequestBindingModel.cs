using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.BindingModels
{
    public class PharmacyRequestBindingModel
    {
        public PharmacyRequestBindingModel()
        {
            Product_Ids = new List<int?>();
        }

        [Required]
        public string Doctor_FirstName { get; set; }

        [Required]
        public string Doctor_LastName { get; set; }

        [Required]
        public string Doctor_Phone { get; set; }

        [Required]
        public string Patient_FirstName { get; set; }

        [Required]
        public string Patient_LastName { get; set; }

        [Required]
        public string Patient_DOB { get; set; }

        [Required]
        public int Gender { get; set; }

        [Required]
        public string Delivery_Address { get; set; }

        [Required]
        public string Delivery_City { get; set; }

        [Required]
        public string Delivery_State { get; set; }

        [Required]
        public string Delivery_Zip { get; set; }

        [Required]
        public string Delivery_Phone { get; set; }

        public List<int?> Product_Ids { get; set; }
    }
}