using BasketWebPanel.Areas.Dashboard.Models;
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
    [Authorize]
    public class DeliverersController : Controller
    {
        // GET: Dashboard/Deliverers
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageDeliverers()
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetDeliveryMen", User, null, true, false, null));

                SearchDeliveryMenViewModel model = new SearchDeliveryMenViewModel();

                if (response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
                else
                    model = response.GetValue("Result").ToObject<SearchDeliveryMenViewModel>();

                foreach (var deliverer in model.DeliveryMen)
                {
                    deliverer.StatusName = deliverer.IsDeleted ? "Blocked" : "Active";
                    if (deliverer.ProfilePictureUrl == null || deliverer.ProfilePictureUrl == "")
                        deliverer.ProfilePictureUrl = "DelivererImages/Default.png";
                }

                model.StatusOptions = Utility.GetUserStatusOptions();

                model.SetSharedData(User);

                return View(model);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDelivererStatuses(List<ChangeDelivererStatusModel> selectedDeliverers)
        {
            try
            {
                if (selectedDeliverers == null)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select a deliverer to save");
                }
                
                ChangeDelivererStatusListModel postModel = new ChangeDelivererStatusListModel();
                postModel.Deliverers = selectedDeliverers;

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/ChangeDelivererStatuses", User, postModel));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                else
                {
                    return Json("Success");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        public ActionResult GetDeliverer(int DelivererId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/User/GetUser", User, null, true, false, null, "UserId=" + DelivererId, "SignInType=1"));

                DelivererBindingModel model = null;

                if (response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
                else
                    model = response.GetValue("Result").ToObject<DelivererBindingModel>();

                if (model.ProfilePictureUrl == null || model.ProfilePictureUrl == "")
                    model.ProfilePictureUrl = "DelivererImages/Default.png";

                model.UserAddresses = model.UserAddresses.Where(x => x.IsDeleted == false).ToList();
                model.SetSharedData(User);

                return View("Deliverer", model);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }
    }
}