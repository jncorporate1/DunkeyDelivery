using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Dashboard/Home
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            //var fullName = claimIdentity.Claims.FirstOrDefault(x => x.Type == "FullName");
            //var profilePictureUrl = claimIdentity.Claims.FirstOrDefault(x => x.Type == "ProfilePictureUrl");

            WebDashboardStatsViewModel model = new WebDashboardStatsViewModel();

            model.SetSharedData(User);

            var response = await ApiCall.CallApi("api/Admin/GetAdminDashboardStats", User, GetRequest: true, parameters: "AdminId=" + model.Id);

            if (response is Error || response == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model = response.GetValue("Result").ToObject<WebDashboardStatsViewModel>();
                User.AddUpdateClaim("UnreadNotificationCount", model.UnreadNotificationsCount.ToString());
            }
            //model.UserName = fullName == null ? "John Doe" : fullName.Value;
            //model.ProfilePictureUrl = profilePictureUrl == null ? "http://10.100.28.44:809/Content/images/img.jpg" : "http://10.100.28.44:809/Content/images/img.jpg";

            model.SetSharedData(User);

            return View(model);

        }


        public ActionResult ResetPassword()
        {
            ResetPasswordBindingModel model = new ResetPasswordBindingModel();
            model.SetSharedData(User);
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/ChangePassword", User, model));

                if (response == null || response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                else
                {
                    model.SetSharedData(User);
                    return RedirectToAction("ResetPassword");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            }

        }
    }
}