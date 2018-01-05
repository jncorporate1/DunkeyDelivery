using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class ProfileViewModel
    {
        public ProfileViewModel(BaseViewModel model,string Password="",string ConfirmPassword="")
        {
            Id = Convert.ToInt32(model.Id);
            FName = model.FirstName;
            LName = model.LastName;
            EmailAddress = model.Email;
            this.Password = Password;
            this.ConfirmPassword = ConfirmPassword;

        }

        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Without Spaces")]
        [Display(Name = "First Name")]
        public string FName { get; set; }

        
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only Without Spaces")]
        [Display(Name = "Last Name")]
        public string LName { get; set; }

    
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

       
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class Profile
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Without Spaces")]
        [Display(Name = "First Name")]
        public string FName { get; set; }


        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only Without Spaces")]
        [Display(Name = "Last Name")]
        public string LName { get; set; }


        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class Addresses : BaseViewModel
    {

        public List<AddressViewModel> addresses { get; set; }
    }
    public class AddressViewModel
    {
        public int Id { get; set; }

        public int User_ID { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }


        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        [MinLength(10, ErrorMessage = "Phone Number is invalid ")]
        [MaxLength(15, ErrorMessage = "Phone Number is invalid")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "Full Address is required")]
        public string FullAddress { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        public string PostalCode { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPrimary { get; set; }
    }
    public class GenericAddresses
    {
        public GenericAddresses()
        {
            addresses = new List<AddressesViewModel>();
        }

        public List<AddressesViewModel> addresses { get; set; }
    }
    public class AddressesViewModel
    {
        public int Id { get; set; }

        public int User_ID { get; set; }

        
        public string City { get; set; }


        public string State { get; set; }

        public string Telephone { get; set; }


        public string FullAddress { get; set; }

        public string PostalCode { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPrimary { get; set; }
    }



    public class CreditCard
    {

        public List<CreditCardViewModel> creditCards { get; set; }
    }
    public class CreditCardViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Card Number is required")]
        [RegularExpression(@"[0-9 ]+", ErrorMessage = "Enter Digits Only")]
        [StringLength(16, ErrorMessage = "Credit Card Number Should Be Of Max Length 16")]
        [MinLength(12,ErrorMessage = "Credit Card Number Should Be Of Min Length 12")]
        [MaxLength(16, ErrorMessage = "Credit Card Number Should Be Of Max Length 16")]

        [DataType(DataType.Text)]
        [Display(Name = "Credit Card")]
        public string CCNo { get; set; }

        [Required(ErrorMessage = "Expiry Date is required")]
        [Display(Name = "Expiry Date")]
        [RegularExpression(@"(0[1-9]|10|11|12)/20[0-9]{2}$", ErrorMessage = "Enter Valid Expiry Date")]
        [DataType(DataType.Text)] 
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        [MaxLength(4, ErrorMessage = "CVV Should Be Of Max Length 4"), MinLength(3, ErrorMessage = "CVV Should Be Of   Min Length 3")]
        [StringLength(4, ErrorMessage = "CVV Should Be Of Max Length 4")]
        [DataType(DataType.Text)]
        [Display(Name = "CVV")]
        public string CCV { get; set; }

        [Required(ErrorMessage = "Billing Postal Code is required")]
        [Display(Name = "Billing Postal Code")]
        [RegularExpression(@"^\d{5}-\d{4}|\d{5}|[A-Z]\d[A - Z] \d[A - Z]\d$", ErrorMessage = "Enter Valid Billing Postal Code")]
        [DataType(DataType.Text)]
        public string BillingCode { get; set; }

        public int User_ID { get; set; }

    }


}