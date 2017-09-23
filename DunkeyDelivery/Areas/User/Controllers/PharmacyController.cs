using DunkeyDelivery.Areas.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitPharmacyForm(PharmacyViewModel model, string returnUrl)
        {
            //model.ConfirmPassword = model.Password;
            //model.Role = 0;
            //model.Phone = "03455249413";

            //if (!ModelState.IsValid)
            //{
            //    return View(ModelState);
            //}

            //var response = await ApiCall<RegisterMerchantViewModel>.CallApi("api/Merchant/Register", model);


            //if (response is Error)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            //}
            //if (response == null)
            //{

            //}




            //ViewBag.Title = "Merchant Sign Up";
            //return View("MerchantSignup");
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}