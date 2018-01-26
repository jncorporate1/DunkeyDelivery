using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class SearchAdminNotificationsViewModel
    {
        public SearchAdminNotificationsViewModel()
        {
            Notifications = new List<AdminNotifications>();
        }
        public List<AdminNotifications> Notifications { get; set; }
    }

    public class SearchSubAdminNotificationsViewModel
    {
        public SearchSubAdminNotificationsViewModel()
        {
            Notifications = new List<AdminSubAdminNotifications>();
        }
        public List<AdminSubAdminNotifications> Notifications { get; set; }
    }
}