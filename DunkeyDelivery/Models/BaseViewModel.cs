using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace DunkeyDelivery.Areas.Dashboard.Models
{
    public abstract class BaseViewModel
    {
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public void SetSharedData(IPrincipal User)
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var fullName = claimIdentity.Claims.FirstOrDefault(x => x.Type == "FullName");
            var Name = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Name");
            var profilePictureUrl = claimIdentity.Claims.FirstOrDefault(x => x.Type == "ProfilePictureUrl");
            UserName = fullName == null ? "" : fullName.Value;
            ProfilePictureUrl =profilePictureUrl==null?"":(Convert.ToString(profilePictureUrl.Value));
            Country =Convert.ToString(claimIdentity.Claims.FirstOrDefault(x => x.Type == "Country"));
        } 
    }
}