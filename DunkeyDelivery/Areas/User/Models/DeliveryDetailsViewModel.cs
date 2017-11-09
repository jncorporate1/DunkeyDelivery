using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    //Testing Comment
    public class DeliveryDetailsViewModel : BaseViewModel
    {
        public Cart Cart { get; set; }
        public DeliveryDetails DeliveryDetails { get; set; }
        public PaymentInormation PaymentInformation { get; set; }
        public string StripeEmail { get; set; }
        public string StripeId { get; set; }
        public double TipAmount { get; set; }
    }

    public class DeliveryDetails
    {
        [Required(ErrorMessage = "First Name is required")]
        [DataType(DataType.Text)]
        //[RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Without Spaces")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        // [RegularExpression(@"^[A-z]+$", ErrorMessage = "Use Alphabets Only Without Spaces")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[0-9*#+]+$", ErrorMessage = "Enter Digits Only")]
        [MinLength(10, ErrorMessage = "Phone Number is invalid ")]
        [MaxLength(15, ErrorMessage = "Phone Number is invalid")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Email Address is invalid")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [DataType(DataType.Text)]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [DataType(DataType.Text)]
        public string City { get; set; }

        [Required(ErrorMessage = "Zip Code is required")]
        [Display(Name = "Zip Code")]
        [RegularExpression(@"^\d{5}-\d{4}|\d{5}|[A-Z]\d[A - Z] \d[A - Z]\d$", ErrorMessage = "Enter Valid Zip Code")]
        [DataType(DataType.Text)]
        public string ZipCode { get; set; }


        public string AdditionalNote { get; set; }

    }
    public class PaymentInormation
    {

        public string PaymentType { get; set; }


        [Required(ErrorMessage = "Card Number is required")]
        [RegularExpression(@"[0-9 ]+", ErrorMessage = "Enter Digits Only")]
        [StringLength(16, ErrorMessage = "Credit Card Number Should Be Of Max Length 16")]
        [DataType(DataType.Text)]
        [Display(Name = "Credit Card")]
        public string CreditCard { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        //  [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "invalid CVV")]
        [MaxLength(4, ErrorMessage = "CVV Should Be Of Max Length 4"), MinLength(3, ErrorMessage = "CVV Should Be Of   Min Length 3")]
        //[StringLength(4, ErrorMessage = "CVV Should Be Of Max Length 4")]
        [DataType(DataType.Text)]
        [Display(Name = "CVV")]
        public string CVV { get; set; }

        [Required(ErrorMessage = "Expiry Date is required")]
        [Display(Name = "Expiry Date")]
        // [RegularExpression(@"(0[1-9]|10|11|12)/20[0-9]{2}$", ErrorMessage = "Enter Valid Expiry Date")]
        [DataType(DataType.Text)]
        public string MonthYear { get; set; }

        [StringLength(4, ErrorMessage = "Tip Should Be Of Max Length 4")]
        [DataType(DataType.Text)]
        [Display(Name = "tip")]
        public string Tip { get; set; }


        [Required(ErrorMessage = "Billing Postal Code is required")]
        [Display(Name = "Billing Postal Code")]
        [RegularExpression(@"^\d{5}-\d{4}|\d{5}|[A-Z]\d[A - Z] \d[A - Z]\d$", ErrorMessage = "Enter Valid Billing Postal Code")]
        [DataType(DataType.Text)]
        public string BillingPostalCode { get; set; }

    }

}