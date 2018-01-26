using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class WebDashboardStatsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalStores { get; set; }
        public int TodayOrders { get; set; }
        public double MonthlyEarning { get; set; }
        public int UnreadNotificationsCount { get; set; }
    }
}