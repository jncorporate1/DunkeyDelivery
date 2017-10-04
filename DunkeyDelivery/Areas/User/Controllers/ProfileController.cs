using DunkeyDelivery.Areas.User.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class ProfileController : Controller
    {
        // GET: User/Profile
        public ActionResult Index()
        {
            ViewBag.BannerImage = "adrees-banner.jpg";
            ViewBag.Title = " Profile";
            ViewBag.BannerTitle = "PROFILE";
            ViewBag.Path = "Home > Profile";
            Global.sharedDataModel.SetSharedData(User);
            return View("Profile", Global.sharedDataModel);
        }

        public async Task<ActionResult> PageView(int id)
        {
            if (id == 0)
            {
                // for Order History 

                return PartialView("_OrderHistory");
                
            }
            else if (id == 1)
            {
                // for recurring orders

                return PartialView("_RecurringOrders");

            }
            else if (id == 2)
            {
                // for account ( Profile View )
                Global.sharedDataModel.SetSharedData(User);
                ProfileViewModel model = new ProfileViewModel(Global.sharedDataModel);
               
                //Global.sharedDataModel.SetSharedData(User);
                return PartialView("_Account",model);

            }
            else if (id == 3)
            {

                // for addresses
                var claimIdentity = ((ClaimsIdentity)User.Identity);
                var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
                var response = await ApiCall<Addresses>.CallApi("api/User/GetUserAddresses?User_id=" + userId, null, false);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

                }
                if (response == null)
                {

                }


                var responseResult = response.GetValue("Result").ToObject<Addresses>();
                responseResult.SetSharedData(User);
                return PartialView("_Addresses",responseResult);

            }
            else if (id == 4)
            {
                // for credit cards
                var claimIdentity = ((ClaimsIdentity)User.Identity);
                var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
                var response = await ApiCall<CreditCard>.CallApi("api/User/GetUserCreditCards?User_id=" + userId, null, false);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

                }
                if (response == null)
                {

                }


                var responseResult = response.GetValue("Result").ToObject<CreditCard>();
             
                return PartialView("_CreditCards", responseResult);

            }else if (id==5)
            {

                return PartialView("_AddAddress",new AddressViewModel());
            }
            else if (id == 6)
            {

                return PartialView("_AddCreditCard", new CreditCardViewModel());
            }
            Global.sharedDataModel.SetSharedData(User);
            return PartialView("_Account",Global.sharedDataModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(Profile model, string returnUrl)
        {
            Profile models = new Profile();
          

            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            var response = await ApiCall<Profile>.CallApi("api/User/EditProfile", model);


            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }
            if (response == null)
            {

            }

            var UpdatedUser= ChangeBaseViewModel(model);
            return Json(UpdatedUser, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAddress(AddressViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("~/Areas/User/Views/Profile/_AddAddress.cshtml", ModelState);
            }
            AddressViewModel models = new AddressViewModel();
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            model.User_ID =Convert.ToInt32(userId);

           

            var response = await ApiCall<AddressViewModel>.CallApi("api/User/AddAddress", model);


            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }
            if (response == null)
            {

            }

          
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCreditcard(CreditCardViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("~/Areas/User/Views/Profile/_AddAddress.cshtml", ModelState);
            }
            CreditCardViewModel models = new CreditCardViewModel();
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            model.User_ID = Convert.ToInt32(userId);



            var response = await ApiCall<CreditCardViewModel>.CallApi("api/User/AddCreditCard", model);


            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }
            if (response == null)
            {

            }


            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeBaseViewModel(Profile model)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                User.AddUpdateClaim("FirstName", model.FName);
                User.AddUpdateClaim("LastName", model.LName);

                User.AddUpdateClaim("FullName", model.FName + " " + model.LName);


                return Json(model);
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

 
        [AllowAnonymous]
        public async Task<ActionResult> RemoveAddress(int id)
        {

            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            var User_ID = Convert.ToInt32(userId);
            var response = await ApiCall<string>.CallApi("api/User/RemoveAddress?address_id=" + id+"&User_Id="+ User_ID, null, false);
           


            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }
            if (response == null)
            {

            }
            var responseShopValue = response.GetValue("Result").ToObject<string>();

            return Json(responseShopValue, JsonRequestBehavior.AllowGet);
        }


    }
}