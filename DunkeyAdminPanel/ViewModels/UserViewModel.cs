using BasketWebPanel.DomainModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel
{
    public class UserViewModel : BaseViewModel, IDisposable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(MyRegularExpressions.Name, ErrorMessage = "Only alphabets are allowed")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        [RegularExpression(MyRegularExpressions.Name, ErrorMessage = "Only alphabets are allowed")]
        public string LastName { get; set; }

        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "PhoneNumber")]
        [StringLength(maximumLength: 15, MinimumLength = 10, ErrorMessage = "Phone Number length should be at least 10 digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The Password and Confirm Password don't match")]
        public string ConfirmPassword { get; set; }
        //public string AccountType { get; set; }

        public short? Status { get; set; }

        public string ZipCode { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string DateofBirth { get; set; }

        public short? SignInType { get; set; }

        public string UserName { get; set; }

        public Token Token { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneConfirmed { get; set; }

        public int StoreId { get; set; }

        //public int RoleId { get; set; }

        public SelectList StoreOptions { get; internal set; }

        public SelectList RoleOptions { get; internal set; }

        public void Dispose()
        {
        }
    }
}