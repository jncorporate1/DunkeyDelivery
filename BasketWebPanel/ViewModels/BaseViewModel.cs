using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.ViewModels
{
    public abstract class BaseViewModel
    {
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int Id { get; set; }
        public int StoreId { get; set; }
        public RoleTypes Role { get; set; }
        public int UnreadNotificationsCount { get; set; }

        public void SetSharedData(IPrincipal User)
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var fullName = claimIdentity.Claims.FirstOrDefault(x => x.Type == "FullName");
            var profilePictureUrl = claimIdentity.Claims.FirstOrDefault(x => x.Type == "ProfilePictureUrl");
            UserName = fullName == null ? "John Doe" : fullName.Value;
            ProfilePictureUrl = string.IsNullOrEmpty(profilePictureUrl.Value) ? "~/Content/images/img.jpg" : profilePictureUrl.Value;
            Id = Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "AdminId").Value);
            Role = (RoleTypes)(Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value));
            StoreId = Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "StoreId").Value);
            UnreadNotificationsCount = Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "UnreadNotificationCount").Value);
        }
    }
}