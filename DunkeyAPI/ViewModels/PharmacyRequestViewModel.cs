using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class PharmacyRequestViewModel
    {
        public PharmacyRequestViewModel()
        {
            PharmacyProducts = new List<ViewModels.PharmacyProductViewModel>();
        }

        public int Id { get; set; }
        public string Doctor_FirstName { get; set; }
        public string Doctor_LastName { get; set; }
        public string Doctor_Phone { get; set; }
        public string Patient_FirstName { get; set; }
        public string Patient_LastName { get; set; }
        public DateTime Patient_DOB { get; set; }
        public int Gender { get; set; }
        public string Delivery_Address { get; set; }
        public string Delivery_City { get; set; }
        public string Delivery_State { get; set; }
        public string Delivery_Zip { get; set; }
        public string Delivery_Phone { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }

        public List<PharmacyProductViewModel> PharmacyProducts { get; set; }
    }

    public class PharmacyProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PharmacyRequestsViewModel
    {
        public PharmacyRequestsViewModel()
        {
            PharmacyRequests = new List<PharmacyRequest>();
        }

        public List<PharmacyRequest> PharmacyRequests { get; set; }
    }
}