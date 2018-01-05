using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class CategoryProductViewModel : BaseViewModel
    {
        public ReviewBindingModel UserReview { get; set; }
        public ReviewViewModel ReviewForView { get; set; }
        public ShopViewModel shopViewModel { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<DeliveryHours> DeliveryHours { get; set; }

    }



    public class LaundryProductsInCategory : BaseViewModel
    {
        public LaundryProductsInCategory()
        {
            laundryproducts = new List<LundryProductVM>();
            StoreViewModel = new ShopViewModel();
            UserReview = new ReviewBindingModel();
            ReviewForView = new ReviewViewModel();
        }
        public ReviewBindingModel UserReview { get; set; }
        public ReviewViewModel ReviewForView { get; set; }
        public List<LundryProductVM> laundryproducts = new List<LundryProductVM>();
        public ShopViewModel StoreViewModel { get; set; }

    }
    public class LundryProductVM
    {
        public LundryProductVM()
        {
            Products = new List<ProductViewModel>();

        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public short Status { get; set; }

        public int Store_Id { get; set; }

        public int? ParentCategoryId { get; set; }

        public List<ProductViewModel> Products { get; set; }

    }

    public class ReviewBindingModel
    {
        public int User_Id { get; set; }
        public int Store_Id { get; set; }
        public int Rating { get; set; }
        public string Feedback { get; set; }
    }
    public class ReviewViewModel
    {
       public List<Review> Reviews { get; set; }
    }
    public class Review
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int Store_Id { get; set; }
        public int Rating { get; set; }
        public string Feedback { get; set; }
        public UserForReviews User { get; set; }
    }

    public class UserForReviews
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string FullName { get; set; } = "";

        public string ProfilePictureUrl { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        //public string AccountType { get; set; }

        public short Status { get; set; }

        public string Country { get; set; } = "";

        public string State { get; set; } = "";

        public string City { get; set; } = "";

        public string Address { get; set; } = "";

        public string Dob { get; set; } = "";

        public short Role { get; set; }

        public string Username { get; set; } = "";
    }
}