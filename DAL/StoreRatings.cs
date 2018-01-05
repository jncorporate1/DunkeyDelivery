using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class StoreRatings
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int User_Id { get; set; }
        [Required]
        public int Store_Id { get; set; }
        [Required]
        public int Rating { get; set; }

        public string Feedback { get; set; }

        public User User { get; set; }
        public Store Store { get; set; }

    }

    public class UserViewModel : IDisposable
    {
        public UserViewModel(User model)
        {
            ID = model.Id;
            FirstName = model.FirstName ?? String.Empty;
            LastName = model.LastName ?? String.Empty;
            FullName = model.FullName ?? String.Empty;
            ProfilePictureUrl = model.ProfilePictureUrl ?? String.Empty;
            Email = model.Email ?? String.Empty;
            Phone = model.Phone ?? String.Empty;
            //AccountType = model.AccountType;
           
        }

        public int ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        //public string AccountType { get; set; }

        public short? Status { get; set; }

        public string ZipCode { get; set; }

        public short is_Notify { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string DateofBirth { get; set; }

        public short? SignInType { get; set; } = 2;

        public string UserName { get; set; }

        //public string Token { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneConfirmed { get; set; }

        //public Token Token { get; set; }
        //public virtual ICollection<CreditCard> CreditCards { get; set; }

        //public virtual ICollection<Favourite> Favourites { get; set; }

        //public virtual ICollection<Notification> Notifications { get; set; }

        //public virtual ICollection<ProductRating> ProductRatings { get; set; }

        public void Dispose()
        {
        }
    }
}
