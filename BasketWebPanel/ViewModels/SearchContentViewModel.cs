using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class SearchContentVM 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int SearchType { get; set; } = 3;

        public short? is_partner { get; set; } = 0;

        public short? is_invester { get; set; } = 0;

        public short? is_press { get; set; } = 0;

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public short is_deleted { get; set; }
    }

    public class ContentVM : BaseViewModel
    {
        public ContentVM()
        {
            Content = new SearchContentVM();
        }
        public SearchContentVM Content { get; set; }
    }

    public class ListContentViewModel : BaseViewModel
    {
        public ListContentViewModel()
        {
            Contributors = new List<SearchContentVM>();
        }
        public List<SearchContentVM> Contributors { get; set; }

    }
}