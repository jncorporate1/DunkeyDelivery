using BasketWebPanel.BindingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class SearchCategoriesViewModel : BaseViewModel
    {
        public SearchCategoriesViewModel()
        {
            Categories = new List<SearchCategoryViewModel>();
        }

        public IEnumerable<SearchCategoryViewModel> Categories { get; set; }
    }
}