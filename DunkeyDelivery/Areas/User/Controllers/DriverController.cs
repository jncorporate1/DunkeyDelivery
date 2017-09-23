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
    public class DriverController : Controller
    {
        // GET: User/Driver
        public ActionResult Index()
        {
            RegisterDriverViewModel model = new RegisterDriverViewModel();
            ViewBag.Title = "Driver";
            return View("~/Areas/User/Views/Driver/DriverIndex.cshtml", model);
        }
         [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DriverRegister(RegisterDriverViewModel model, string returnUrl)
        {
            //model.ConfirmPassword = model.Password;
            //model.Role = 0;
            //model.Phone = "03455249413";

            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            var response = await ApiCall<RegisterDriverViewModel>.CallApi("api/Driver/Register", model);


            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }
            if(response == null){
               
            }
           



            ViewBag.Title = "Merchant Sign Up";
            return View("~/Areas/User/Views/Driver/DriverIndex.cshtml", model);
        }
    }
}