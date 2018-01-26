using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class SearchDeliveryMenViewModel
    {
        //public SearchDeliveryMenViewModel()
        //{
        //    DeliveryMen = new List<DeliveryMan>();
        //}
        //public List<DeliveryMan> DeliveryMen { get; set; }
    }
    public class SearchUsersViewModel
    {
        public SearchUsersViewModel()
        {
            Users = new List<User>();
        }
        public List<User> Users { get; set; }
    }
}