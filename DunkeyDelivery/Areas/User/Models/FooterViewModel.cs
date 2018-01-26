using DunkeyDelivery.Areas.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class FooterViewModel
    {
    }
    public class FAQListViewModel : BaseViewModel
    {
        public FAQListViewModel()
        {
            FAQs = new List<FAQViewModel>();
        }
        public List<FAQViewModel> FAQs { get; set; }
    }
    public class FAQViewModel
    {

        public int Id { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string Type { get; set; }

    }


    public class FAQListPartialViewModel
    {
        public FAQListPartialViewModel()
        {
            FAQs = new List<FAQViewModel>();
        }
        public List<FAQViewModel> FAQs { get; set; }
        public int TotalRecords { get; set; }
    }
}