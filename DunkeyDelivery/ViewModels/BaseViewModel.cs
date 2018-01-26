using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.ViewModels
{
    public abstract class BaseViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; } 
        public string Email { get; set; } 
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Country { get; set; }

        public void SetSharedData(IPrincipal User)
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var idClaim = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id");
            var FirstNameClaim = claimIdentity.Claims.FirstOrDefault(x => x.Type == "FirstName");
            var EmailClaim = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Email");
            var LastNameClaim = claimIdentity.Claims.FirstOrDefault(x => x.Type == "LastName");
            var fullName = claimIdentity.Claims.FirstOrDefault(x => x.Type == "FullName");
            var profilePictureUrl = claimIdentity.Claims.FirstOrDefault(x => x.Type == "ProfilePictureUrl");
            var CountryClaim = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Country");
            if (idClaim != null)
            {
                Id = idClaim.Value;
                FirstName = FirstNameClaim.Value;
                LastName = LastNameClaim.Value;
                Email = EmailClaim.Value;
                UserName = fullName.Value == null ? "" : fullName.Value;
                ProfilePictureUrl = profilePictureUrl == null ? "http://10.100.28.38:810/Content/images/img.jpg" : "http://10.100.28.38:810/Content/images/img.jpg";
                Country = CountryClaim==null?"USA":CountryClaim.Value;
            }
            Country = CountryClaim == null ? "USA" : CountryClaim.Value;




        }
    }
}