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
    public class MerchantController : Controller
    {
        // GET: Merchant
        public ActionResult Index()
        {
            Global.sharedDataModel.SetSharedData(User);
            ViewBag.Title = "Merchants";
            return View("MerchantIndex",Global.sharedDataModel);
        }
        public ActionResult Register()
        {
            RegisterMerchantViewModel model = new RegisterMerchantViewModel();
            model.SetSharedData(User);
            ViewBag.Title = "Merchant Sign Up";
            return View("MerchantSignup",model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MerchantRegister(RegisterMerchantViewModel model, string returnUrl)
        {
            RegisterMerchantViewModel models = new RegisterMerchantViewModel();
             //model.ConfirmPassword = model.Password;
            model.Role = Convert.ToInt16(Utility.RoleTypes.SubAdmin);
            model.Status = 0;
            //model.Phone = "03455249413";

            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            var response = await ApiCall<RegisterMerchantViewModel>.CallApi("api/Merchant/Register", model);


            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }
            if(response == null){
               
            }



            
            ViewBag.Title = "Merchant Sign Up";
            return View("MerchantSignup", models);
         }
    }
}