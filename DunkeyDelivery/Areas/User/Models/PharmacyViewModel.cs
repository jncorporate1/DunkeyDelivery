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

        [Required(ErrorMessage ="Medication is required")]
        public string medication { get; set; }

        [Required(ErrorMessage = "Doctor First Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Without Spaces")]
        [Display(Name = "Doctor First Name")]
        public string DoctorFirstName { get; set; }


        [Required(ErrorMessage = "Doctor Last Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only Without Spaces")]
        [Display(Name = "Doctor Last Name")]
        public string DoctorLastName { get; set; }

        [Required(ErrorMessage = "Doctor Phone Number is required")]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        [MinLength(10, ErrorMessage = "Doctor Phone Number is invalid ")]
        [MaxLength(15, ErrorMessage = "Doctor Phone Number is invalid")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Doctor Phone Number")]
        public string DoctorPhone { get; set; }



        [Required(ErrorMessage = "Patient First Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Without Spaces")]
        [Display(Name = "Patient First Name")]
        public string PatientFirstName { get; set; }


        [Required(ErrorMessage = "Patient Last Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only Without Spaces")]
        [Display(Name = "Patient Last Name")]
        public string PatientLastName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [DataType(DataType.Text)]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Date Of Birth is required")]
        [RegularExpression(@"([0-9]{2})\/([0-9]{2})\/([0-9]{4})", ErrorMessage = "invalid Date Of Birth")]
        public string DateOfBirth { get; set; }


        [Required(ErrorMessage = "Address is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [DataType(DataType.Text)]
        [Display(Name = "City")]
        public string City { get; set; }


        [Required(ErrorMessage = "State is required")]
        [DataType(DataType.Text)]
        [Display(Name = "State")]
        public string State { get; set; }


        [Required(ErrorMessage = "Zip Code is required")]
        [Display(Name = "Zip Code")]
        [RegularExpression(@"^\d{5}-\d{4}|\d{5}|[A-Z]\d[A - Z] \d[A - Z]\d$", ErrorMessage = "Enter Valid Zip Code")]
        [DataType(DataType.Text)]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Delivery Phone is required")]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        [MinLength(10, ErrorMessage = "Phone Number is invalid ")]
        [MaxLength(15, ErrorMessage = "Phone Number is invalid")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Delivery Phone Number")]
        public string DeliveryPhone { get; set; }


        public ShopViewModel storeViewModel { get; set; }

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