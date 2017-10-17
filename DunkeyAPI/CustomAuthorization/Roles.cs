using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.CustomAuthorization
{
    public class RoleTypes
    {

        public const string User = "User";
        public const string SubAdmin = "SubAdmin";
        public const string SuperAdmin = "SuperAdmin";
        public const string ApplicationAdmin = "ApplicationAdmin";
    }

    public enum RolesCode
    {
        User = 0,
        Deliverer = 1,
        SubAdmin = 2,
        SuperAdmin = 3,
        ApplicationAdmin = 4
    }
}