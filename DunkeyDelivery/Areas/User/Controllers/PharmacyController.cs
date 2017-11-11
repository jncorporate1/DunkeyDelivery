using DunkeyDelivery.Areas.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class PharmacyController : Controller
    {
        // GET: User/Pharmacy
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PharmacyDetails()
        {
            return View();
        }


        [AllowAnonymous]
        public async Task<ActionResult> MedicationNames(int Store_Id, string Name="")
        {
            Medications responseResult = new Medications();
            var response = await ApiCall<Medications>.CallApi("api/Products/GetMedicationNames?Store_Id="+Store_Id+ "&search_string=" + Name, null, false);
            responseResult = response.GetValue("Result").ToObject<Medications>();
            return Json(responseResult, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitPharmacyForm(PharmacyViewModel model, string returnUrl)
        {
         

            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            var response = await ApiCall<PharmacyViewModel>.CallApi("api/Pharmacy/SubmitPharmacyRequest", model);


            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }
            if (response == null)
            {

            }




            ViewBag.Title = "Merchant Sign Up";
            return View("MerchantSignup");
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}