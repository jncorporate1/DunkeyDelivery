using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class FAQViewModel
    {

        public int Id { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string Type { get; set; }
    }
}