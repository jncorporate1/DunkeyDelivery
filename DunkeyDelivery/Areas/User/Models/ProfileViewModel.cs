using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class ProfileViewModel : BaseViewModel
    {

        public int Id { get; set; }

        public string FName { get; set; }

        public string EmailAddress { get; set; }

        public string LName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

    }
}