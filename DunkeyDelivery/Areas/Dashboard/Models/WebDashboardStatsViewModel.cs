using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.Dashboard.Models
{
    public class WebDashboardStatsViewModel : BaseViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalStores { get; set; }
    }
}