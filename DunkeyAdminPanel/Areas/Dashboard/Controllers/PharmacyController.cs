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
    public class PharmacyController : Controller
    {
        // GET: Dashboard/Pharmacy
        public ActionResult Index(int? Id)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Pharmacy/GetSinglePharmacyRequest", User, null, true, false, null, "Id=" + Id));

                PharmacyRequestViewModel model = null;

                if (response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
                else
                    model = response.GetValue("Result").ToObject<PharmacyRequestViewModel>();

                model.SetSharedData(User);

                return View("Index", model);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }

        public ActionResult ManagePharmacy()
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Pharmacy/GetPharmacyRequests", User, null, true, false, null));

            SearchPharmacyRequestsViewModel model = new SearchPharmacyRequestsViewModel();

            if (response == null || response is Error)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error");
            else
                model = response.GetValue("Result").ToObject<SearchPharmacyRequestsViewModel>();

            model.StatusOptions = Utility.GetPharmacyRequestStatusOptions();

            foreach (var pharmacyRequest in model.PharmacyRequests)
            {
                switch (pharmacyRequest.Status)
                {
                    case 1:
                        pharmacyRequest.StatusName = "Accepted";
                        break;
                    case 2:
                        pharmacyRequest.StatusName = "Rejected";
                        break;
                    default:
                        pharmacyRequest.StatusName = "Initial";
                        break;
                }
            }

            model.SetSharedData(User);

            return View("ManagePharmacyRequests", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePharmacyStatuses(List<ChangePharmacyStatusModel> selectedRequests)
        {
            try
            {
                if (selectedRequests == null)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select a request to save");
                }

                ChangePharmacyStatusListModel postModel = new ChangePharmacyStatusListModel();
                postModel.PharmacyRequests = selectedRequests;

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/ChangePharmacyRequestStatuses", User, postModel));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                else
                    return Json("Success");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }
    }
}