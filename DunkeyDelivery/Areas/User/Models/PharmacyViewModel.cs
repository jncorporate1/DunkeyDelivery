using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class PharmacyViewModel : BaseViewModel
    {

        public PharmacyViewModel()
        {
            Product_Ids = new List<int>();
        }

        public List<int> Product_Ids { get; set; }

        [Required(ErrorMessage = "Doctor First Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Without Spaces")]
        [Display(Name = "Doctor First Name")]
        public string Doctor_FirstName { get; set; }
    


        [Required(ErrorMessage = "Doctor Last Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only Without Spaces")]
        [Display(Name = "Doctor Last Name")]
        public string Doctor_LastName { get; set; }

        [Required(ErrorMessage = "Doctor Phone Number is required")]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        [MinLength(10, ErrorMessage = "Doctor Phone Number is invalid ")]
        [MaxLength(15, ErrorMessage = "Doctor Phone Number is invalid")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Doctor Phone Number")]
        public string Doctor_Phone { get; set; }



        [Required(ErrorMessage = "Patient First Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Without Spaces")]
        [Display(Name = "Patient First Name")]
        public string Patient_FirstName { get; set; }


        [Required(ErrorMessage = "Patient Last Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only Without Spaces")]
        [Display(Name = "Patient Last Name")]
        public string Patient_LastName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [DataType(DataType.Text)]
        public int Gender { get; set; }
        
        [Required(ErrorMessage = "Date Of Birth is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime dtpPatient_DOB { get; set; }

        public string Patient_DOB { get; set; }



        [Required(ErrorMessage = "Address is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Address")]
        public string Delivery_Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [DataType(DataType.Text)]
        [Display(Name = "City")]
        public string Delivery_City { get; set; }


        [Required(ErrorMessage = "State is required")]
        [DataType(DataType.Text)]
        [Display(Name = "State")]
        public string Delivery_State { get; set; }


        [Required(ErrorMessage = "Zip Code is required")]
        [Display(Name = "Zip Code")]
        [RegularExpression(@"^\d{5}-\d{4}|\d{5}|[A-Z]\d[A - Z] \d[A - Z]\d$", ErrorMessage = "Enter Valid Zip Code")]
        [DataType(DataType.Text)]
        public string Delivery_Zip { get; set; }

        [Required(ErrorMessage = "Delivery Phone is required")]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        [MinLength(10, ErrorMessage = "Phone Number is invalid ")]
        [MaxLength(15, ErrorMessage = "Phone Number is invalid")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Delivery Phone Number")]
        public string Delivery_Phone { get; set; }


        public ShopViewModel storeViewModel { get; set; }

    }
    public class MedicationList
    {
        public int Id { get; set;}

        [Required(ErrorMessage = "Medication is required")]
        public string medication { get; set; }
    }
    public class MedicationNames
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public int Store_Id { get; set; }

        public string Size { get; set; }

    }

    public class Medications
    {
        public Medications()
        {
            medications = new List<MedicationNames>();
        }
        public List<MedicationNames> medications;
    }
}