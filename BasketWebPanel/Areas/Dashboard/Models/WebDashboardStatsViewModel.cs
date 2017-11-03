using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class WebDashboardStatsViewModel : BaseViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalStores { get; set; }
        public int TotalUsers { get; set; }
        public int TodayOrders { get; set; }
        public double MonthlyEarning { get; set; }
    }
}