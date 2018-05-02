using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class SearchBlogViewModel : BaseViewModel
    {
        public SearchBlogViewModel()
        {
            BlogList = new List<BlogsViewModel>();
        }
        public List<BlogsViewModel> BlogList { get; set; }

    }

    public class BlogsViewModel
    {
  
        public int Id { get; set; }

        public string Title { get; set; }

        public string CategoryType { get; set; }

        public string ImageUrl { get; set; }

        public int Admin_ID { get; set; }

        public string Description { get; set; }

        public DateTime DateOfPosting { get; set; }

        public short is_popular { get; set; }

        public short? is_deleted { get; set; } = 0;

        public int TotalComments { get; set; } = 0;

        public bool ImageDeletedOnEdit { get; set; } = false;

        public string Email { get; set; } = "";

    }
}