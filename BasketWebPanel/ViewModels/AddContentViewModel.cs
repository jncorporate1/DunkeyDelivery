using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class AddContentViewModel : BaseViewModel
    {
        public AddContentViewModel()
        {
            Content = new ContentViewModel();
        }
        public ContentViewModel Content { get; set; }

    }

    public class ContentListViewModel : BaseViewModel
    {

        public List<ContentViewModel> ContentList { get; set; }
    }
    public class ContentViewModel
    {
        public ContentViewModel()
        {
            Images = new List<Images>();
        }
        public int Id { get; set; }

        public short? is_partner { get; set; } = 0;

        public short? is_invester { get; set; } = 0;

        public short? is_press { get; set; } = 0;

        public List<Images> Images { get; set; }
        
        public string Description { get; set; }

        public short is_deleted { get; set; }
    }
    public class Images
    {
        public string Image { get; set; }
    }

}