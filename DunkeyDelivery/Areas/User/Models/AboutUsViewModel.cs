using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class AboutUsViewModel : BaseViewModel
    {
        public int Id { get; set; } = 0;

        public string Heading { get; set; } = "";

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public string VideoUrl { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        public int Type { get; set; }
    }
}