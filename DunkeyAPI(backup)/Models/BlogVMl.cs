using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class BlogVMl
    {
    }

    public class BlogPostListViewModel
    {
        public BlogPostListViewModel()
        {
            BlogPosts = new List<BlogPosts>();
        }
        public List<BlogPosts> BlogPosts { get; set; }
        public List<popularCategories> popularCategories { get; set; }

    }

    public class popularCategories
    {
        public string CategoryType { get; set; }
        public int TotalCount { get; set; }
    }

    public class SearchBlogViewModel
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

        public string Email { get; set; }
    }

    public class SearchBlogListViewModel
    {
        public SearchBlogListViewModel()
        {
            BlogList = new List<SearchBlogViewModel>();
        }
             
        public List<SearchBlogViewModel> BlogList { get; set; }
    }
}