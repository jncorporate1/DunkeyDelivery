using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class AddNotificationsViewModel : BaseViewModel
    {
        public AddNotificationsViewModel()
        {
            TargetOptions = new SelectList(new List<SelectListItem>());
            Notification = new NotificationBindingModel();
        }

        public SelectList TargetOptions { get; set; }

        public NotificationBindingModel Notification;
        
    }
    public class AddNotificationsViewModel1 : BaseViewModel
    {
        public AddNotificationsViewModel1()
        {
            TargetOptions = new SelectList(new List<SelectListItem>());
            Notification = new NotificationBindingModel1();
        }

        public SelectList TargetOptions { get; set; }

        public NotificationBindingModel1 Notification;
    }
    public class NotificationBindingModel1
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "This field is requried")]
        public string Description { get; set; } = "";

        public int TargetAudience { get; set; }

        public string ImageUrl { get; set; }
    }
}