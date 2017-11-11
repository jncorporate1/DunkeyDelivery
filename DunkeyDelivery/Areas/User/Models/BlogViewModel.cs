using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{

    
    public class BlogViewModel : BaseViewModel
    {

        public List<PostsViewModel> BlogPosts { get; set; }
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

        public int TotalComments { get; set; }

        public DateTime? DateOfPosting { get; set; }

        public Admin Admin { get; set; }
    }
    public class Admin
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
    // seperate for details page 
    public class BlogDetailsViewModel : BaseViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string CategoryType { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public int User_ID { get; set; }

        public short is_popular { get; set; }

        public int TotalComments { get; set; }

        public DateTime? DateOfPosting { get; set; }

        public List<BlogComments> BlogComments { get; set; }

        public Admin Admin { get; set; }
    }
    public class BlogComments
    {
        public int Id { get; set;}
        
        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime PostedDate { get; set; }

        public int Post_Id { get; set; }

        public int User_Id { get; set; }

        public User User { get; set; }
        
    }

    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }
    }

    public class CommentViewModel
    {
        public int Id { get; set; }

        public int User_Id { get; set; }

        public string Message { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? PostedDate { get; set; }

        public int Post_Id { get; set; }


    }
}