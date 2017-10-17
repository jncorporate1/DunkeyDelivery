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

    public class ChangeOrderStatusListModel
    {
        public ChangeOrderStatusListModel()
        {
            Orders = new List<ChangeOrderStatusModel>();
        }
        public List<ChangeOrderStatusModel> Orders { get; set; }
    }
}