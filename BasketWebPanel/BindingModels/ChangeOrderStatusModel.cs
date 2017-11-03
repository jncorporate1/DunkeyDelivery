using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    public class ChangeOrderStatusModel
    {
        public int OrderId { get; set; }
        public int StoreOrder_Id { get; set; }
        public int Status { get; set; }
    }

    public class ChangeDelivererStatusModel
    {
        public int DelivererId { get; set; }
        public bool Status { get; set; }
    }

    public class ChangeUserStatusModel
    {
        public int UserId { get; set; }
        public bool Status { get; set; }
    }

    public class ChangeUserStatusListModel
    {
        public ChangeUserStatusListModel()
        {
            Users = new List<ChangeUserStatusModel>();
        }

        public List<ChangeUserStatusModel> Users { get; set; }
    }

    public class ChangeOrderStatusListModel
    {
        public ChangeOrderStatusListModel()
        {
            Orders = new List<ChangeOrderStatusModel>();
        }
        public List<ChangeOrderStatusModel> Orders { get; set; }
    }

    public class ChangeDelivererStatusListModel
    {
        public ChangeDelivererStatusListModel()
        {
            Deliverers = new List<ChangeDelivererStatusModel>();
        }
        public List<ChangeDelivererStatusModel> Deliverers { get; set; }
    }
}