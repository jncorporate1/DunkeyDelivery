using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    public class NotificationBindingModel : BaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="This field is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "This field is requried")]
        public string Description { get; set; } = "";
        
        public int TargetAudience { get; set; }

        public string ImageUrl { get; set; }
    }
}