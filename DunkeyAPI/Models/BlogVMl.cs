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
}