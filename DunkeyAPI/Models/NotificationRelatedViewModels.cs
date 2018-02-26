using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class NotificationRelatedViewModels
    {
    }
    public class RegisterPushNotificationBindingModel
    {
        public string DeviceName { get; set; }

        [Required]
        public string UDID { get; set; }

        [Required]
        public bool IsAndroidPlatform { get; set; }

        [Required]
        public bool IsPlayStore { get; set; }

        [Required]
        public int User_Id { get; set; }

        [Required]
        public bool IsProduction { get; set; }

        public string AuthToken { get; set; }

        [Required]
        public int SignInType { get; set; }
    }

    public class NotificationsViewModel
    {
        public IEnumerable<Notification> Notifications { get; set; }
        public int TotalNotifications { get; set; }
    }
    public class NotificationVM
    {
        public int NotificationCount { get; set; }
    }
}