using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class AddBlogViewModel : BaseViewModel
    {
        public AddBlogViewModel()
        {
            Blog = new BlogViewModel();
        }
        public BlogViewModel Blog { get; set; }

       
    }

    public class BlogViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string CategoryType { get; set; }

        
        public string ImageUrl { get; set; }

        public int User_ID { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DateOfPosting { get; set; }

        public short is_popular { get; set; }

    }
}