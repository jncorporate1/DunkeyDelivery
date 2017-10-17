using BasketWebPanel.Areas.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class StoresListViewModel : BaseViewModel
    {
        public StoresListViewModel()
        {
            Stores = new List<StoreViewModel>();
        }
        public List<StoreViewModel> Stores { get; set; }
    }
}