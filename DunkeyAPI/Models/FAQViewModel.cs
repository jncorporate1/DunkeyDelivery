using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class FAQViewModel
    {
        public FAQViewModel()
        {
            FAQs = new List<FAQ>();
        }
        public List<FAQ> FAQs { get; set; }
        public int TotalRecords { get; set; }
    }
}