using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.BindingModels
{
    public class ChangeOrderStatusBindingModel
    {
        public int OrderId { get; set; }
        public int StoreOrder_Id { get; set; }
        public int Status { get; set; }
    }

    public class ChangeOrderStatusListBindingModel
    {
        public ChangeOrderStatusListBindingModel()
        {
            Orders = new List<ChangeOrderStatusBindingModel>();
        }
        public List<ChangeOrderStatusBindingModel> Orders { get; set; }
    }

    public class ChangePharmacyStatusBindingModel
    {
        public int PharmacyId { get; set; }
        public int Status { get; set; }
    }

    public class ChangePharmacyStatusListBindingModel
    {
        public ChangePharmacyStatusListBindingModel()
        {
            PharmacyRequests = new List<ChangePharmacyStatusBindingModel>();
        }
        public List<ChangePharmacyStatusBindingModel> PharmacyRequests { get; set; }
    }

    public class ChangeUserStatusBindingModel
    {
        public int UserId { get; set; }
        public bool Status { get; set; }
    }
    public class ChangeUserStatusListBindingModel
    {
        public ChangeUserStatusListBindingModel()
        {
            Users = new List<ChangeUserStatusBindingModel>();
        }
        public List<ChangeUserStatusBindingModel> Users { get; set; }
    }
}