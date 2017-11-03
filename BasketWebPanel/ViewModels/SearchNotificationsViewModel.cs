using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class SearchNotificationsViewModel : BaseViewModel
    {
        public SearchNotificationsViewModel()
        {
            Notifications = new List<SearchNotificationViewModel>();
        }

        public List<SearchNotificationViewModel> Notifications { get; set; }
    }

    public class SearchNotificationViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int TargetAudienceType { get; set; }

        public string TargetAudience { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}