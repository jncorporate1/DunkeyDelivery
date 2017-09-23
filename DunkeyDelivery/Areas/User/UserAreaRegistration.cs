using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User
{
    public class UserAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "User";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "User_default",
                "User/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            //context.MapRoute(
            //    "User_default",
            //    "User/{controller}/{action}/{id}",
            //    new {Controller = "Home", action = "Index", id = UrlParameter.Optional },
            //namespaces: new[] { "DunkeyDelivery.Areas.User.Controllers" }

             
            //new[] { "DunkeyDelivery.User.Controllers" }
            //);
        }
    }
}