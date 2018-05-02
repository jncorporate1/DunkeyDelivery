using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class AddUnitViewModel : BaseViewModel
    {
        public AddUnitViewModel()
        {
            UnitOptions = new SelectList(new List<SelectListItem>());
            Size = new SizeBindingModel();
        }

        public SelectList UnitOptions { get; internal set; }

        public SizeBindingModel Size { get; set; }

    }
}