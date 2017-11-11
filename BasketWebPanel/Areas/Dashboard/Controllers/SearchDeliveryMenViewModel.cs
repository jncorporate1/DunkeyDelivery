using BasketWebPanel.CustomJson;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Security.Principal;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{

    public class SearchDeliveryMenViewModel : BaseViewModel
    {
        public SearchDeliveryMenViewModel()
        {
            DeliveryMen = new List<DeliveryManBindingModel>();
        }
        public List<DeliveryManBindingModel> DeliveryMen { get; set; }

        public SelectList StatusOptions { get; internal set; }
    }

    public class DeliveryManBindingModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ZipCode { get; set; }

        public string DateOfBirth { get; set; }

        public string Phone { get; set; }

        public string ProfilePictureUrl { get; set; }
        
        public short SignInType { get; set; }

        public bool IsChecked { get; set; }


        //public virtual Store Store { get; set; }
        public bool IsOnline { get; set; }
        public short? Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneConfirmed { get; set; }

        public bool IsDeleted { get; set; }

        public string StatusName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [JsonConverter(typeof(DbGeographyConverter))]
        public DbGeography Location { get; set; }

        public short Type { get; set; }

    }
}