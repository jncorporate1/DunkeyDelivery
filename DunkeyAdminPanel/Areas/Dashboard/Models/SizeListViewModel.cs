using BasketWebPanel.BindingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class SizeListViewModel
    {
        public SizeListViewModel()
        {
            SizeList = new List<SizeBindingModel>();
        }
        public List<SizeBindingModel> SizeList { get; set; }
    }
    
}