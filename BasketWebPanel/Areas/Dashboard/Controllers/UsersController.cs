using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    public class UsersController : Controller
    {
        // GET: Dashboard/Users
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageUsers()
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetUsers", User, null, true, false, null));

            SearchUserViewModel model = new SearchUserViewModel();

            if (response is Error)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            else
                model = response.GetValue("Result").ToObject<SearchUserViewModel>();

            foreach (var user in model.Users)
            {
                user.StatusName = user.IsDeleted ? "Blocked" : "Active";
                if (user.ProfilePictureUrl == null || user.ProfilePictureUrl == "")
                    user.ProfilePictureUrl = "UserImages/Default.png";
            }
            model.StatusOptions = Utility.GetUserStatusOptions();

            model.SetSharedData(User);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveUserStatuses(List<ChangeUserStatusModel> selectedUsers)
        {
            try
            {
                if (selectedUsers == null)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select a user to save");
                }

                ChangeUserStatusListModel postModel = new ChangeUserStatusListModel();
                postModel.Users = selectedUsers;

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/ChangeUserStatuses", User, postModel));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                else
                {
                    return Json("Success");
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }

        public ActionResult GetUser(int UserId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/User/GetUser", User, null, true, false, null, "UserId="+ UserId, "SignInType=0"));

                UserBindingModel model = null;

                if (response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
                else
                    model = response.GetValue("Result").ToObject<UserBindingModel>();

                if (model.ProfilePictureUrl == null || model.ProfilePictureUrl == "")
                    model.ProfilePictureUrl = "UserImages/Default.png";

                model.UserAddresses = model.UserAddresses.Where(x => x.IsDeleted == false).ToList();
                model.PaymentCards = model.PaymentCards.Where(x => x.IsDeleted == false).ToList();
                model.SetSharedData(User);

                return View("User", model);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }
    }

}