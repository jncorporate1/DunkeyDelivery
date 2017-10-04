using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class BlogViewModel : BaseViewModel
    {
        
        public List<PostsViewModel> postsViewModel { get; set; }
    }
    public class PostsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string CategoryType { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public int User_ID { get; set; }

        public short is_popular { get; set; }

        public DateTime DateOfPosting { get; set; }
    }
}