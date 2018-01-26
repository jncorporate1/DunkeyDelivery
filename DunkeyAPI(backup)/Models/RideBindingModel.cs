using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class RideBindingModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string BusinessName { get; set; }

        public string BusinessType { get; set; }

        public string Email { get; set; }

        public string ZipCode { get; set; }

        public string Phone { get; set; }

        public int Status { get; set; }

        public int SignInType { get; set; }

    }
}