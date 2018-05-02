using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        // GET: Dashboard/Notifications
        public ActionResult Index(int? NotificationId)
        {
            AddNotificationsViewModel model = new AddNotificationsViewModel();

            List<SelectListItem> options = new List<SelectListItem>();

            //options.Add(new SelectListItem { Text = "Users & Deliverers", Value = "1" });
            //options.Add(new SelectListItem { Text = "Users Only", Value = "2" });
            //options.Add(new SelectListItem { Text = "Deliverers Only", Value = "3" });
            options.Add(new SelectListItem { Text = "Sub Admins", Value = "4" });

            model.TargetOptions = new SelectList(options);

            model.SetSharedData(User);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NotificationBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/AddNotification", User, model));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                else
                {
                    TempData["SuccessMessage"] = "The notification has been sent successfully.";
                    model.SetSharedData(User);
                    return Json("Success");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex));
            }
        }

        public ActionResult MyNotificationsJson()
        {
            try
            {   
                MyNotificationsViewModel model = new MyNotificationsViewModel();
                model.SetSharedData(User);
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetMyNotifications", User, null, true, false, null, "Id=" + model.Id, "Unread=true"));
                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
                }
                else
                {
                    model = response.GetValue("Result").ToObject<MyNotificationsViewModel>();
                    foreach (var notification in model.Notifications)
                    {
                        var timeDifference = DateTime.Now.Subtract(notification.CreatedDate);
                        if (timeDifference.Days > 0)
                            notification.TimeText = timeDifference.Days + (timeDifference.Days == 1 ? " day ago" : " days ago");
                        else if (timeDifference.Hours > 0)
                            notification.TimeText = timeDifference.Hours + (timeDifference.Hours == 1 ? " hour ago" : " hours ago");
                        else
                            notification.TimeText = timeDifference.Minutes + (timeDifference.Minutes <= 1 ? " minute ago" : " minutes ago");
                    }
                    User.AddUpdateClaim("UnreadNotificationCount", model.Notifications.Count.ToString());
                    return Json(new { Notifications = model.Notifications }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex));
            }
        }

        public ActionResult MyNotifications()
        {
            try
            {
                MyNotificationsViewModel model = new MyNotificationsViewModel();

                model.SetSharedData(User);

                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetMyNotifications", User, null, true, false, null, "Id=" + model.Id));

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
                }
                else
                {
                    model = response.GetValue("Result").ToObject<MyNotificationsViewModel>();
                }
                model.SetSharedData(User);
                return View(model);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex));
            }
        }

        public ActionResult ManageNotifications()
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchNotifications", User, null, true, false, null));

            SearchNotificationsViewModel model = new SearchNotificationsViewModel();

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
            else
            {
                model = response.GetValue("Result").ToObject<SearchNotificationsViewModel>();
            }

            foreach (var notification in model.Notifications)
            {
                switch (notification.TargetAudienceType)
                {
                    case 1:
                        notification.TargetAudience = "Users & Deliverers";
                        break;
                    case 2:
                        notification.TargetAudience = "Users Only";
                        break;
                    case 3:
                        notification.TargetAudience = "Deliverers Only";
                        break;
                    case 4:
                        notification.TargetAudience = "Sub Admins";
                        break;
                }
            }

            model.SetSharedData(User);

            return View(model);
        }

        public ActionResult MarkNotificationAsRead(int NotificationId)
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var AdminId = Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "AdminId").Value);

            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/MarkNotificationAsRead", User, null, true, false, null, "Id=" + NotificationId, "AdminId=" + AdminId));

            if (response is Error)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            else
            {
                var unreadNotificationCount = response.GetValue("Result").ToObject<string>();
                User.AddUpdateClaim("UnreadNotificationCount", unreadNotificationCount);
                return Json(new { UnreadNotificationCount = unreadNotificationCount }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}