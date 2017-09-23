using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class UserViewModel:IDisposable
    {
        public UserViewModel(User model)
        {
            Id = model.Id;
            FirstName = model.FirstName;
            LastName = model.LastName;
            FullName = model.FullName;
            Email = model.Email;
            Phone = model.Phone;
            Address = model.Address;
            City = model.City;
            State = model.State;
            Role = model.Role;
            Username = model.Username;
            ProfilePictureUrl = model.ProfilePictureUrl;
        }
        public int Id { get; set; }

     
        public string FirstName { get; set; }

       
        public string LastName { get; set; }

   
        public string FullName { get; set; }

       
        public string Email { get; set; }

         
        public string Phone { get; set; }

         
        public string Address { get; set; }

         
        public string City { get; set; }

         
        public string State { get; set; }

         
        public string Country { get; set; }

         
        public string Dob { get; set; }

         
        public short Role { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Username { get; set; }
        public object Token { get; internal set; }

        public void Dispose()
        {
        }
    }


    public class AdminViewModel : IDisposable
    {
        public AdminViewModel(Admin model)
        {
            Id = model.Id;
            FirstName = model.FirstName;
            LastName = model.LastName;
            FullName = model.FullName;
            Email = model.Email;
            Phone = model.Phone;
            BusinessName = model.BusinessName;
            BusinessType = model.BusinessType;
            Role = model.Role;
            ZipCode = model.ZipCode;
            Status = model.Status;
       
         
        }
        public int Id { get; set; }


        public string FirstName { get; set; }

        public string BusinessName { get; set; }

        public string BusinessType { get; set; }


        public string LastName { get; set; }


        public string FullName { get; set; }


        public string Email { get; set; }

        public string ZipCode { get; set; }

        public short? Status { get; set; }
        public string Phone { get; set; }

        public short Role { get; set; }

        public string ProfilePictureUrl { get; set; }


        public object Token { get; internal set; }

        public void Dispose()
        {
        }
    }


    public class RideViewModel : IDisposable
    {
        public RideViewModel(Rider model)
        {
            Id = model.Id;
       
            FullName = model.FullName;
            Email = model.Email;
            Phone = model.Phone;
            BusinessName = model.BusinessName;
            BusinessType = model.BusinessType;
           
            ZipCode = model.ZipCode;
            Status = model.Status;


        }
        public int Id { get; set; }

        
        public string BusinessName { get; set; }

        public string BusinessType { get; set; }
        

        public string FullName { get; set; }


        public string Email { get; set; }

        public string ZipCode { get; set; }

        public short? Status { get; set; }

        public string Phone { get; set; }


        public object Token { get; internal set; }

        public void Dispose()
        {
        }
    }

}