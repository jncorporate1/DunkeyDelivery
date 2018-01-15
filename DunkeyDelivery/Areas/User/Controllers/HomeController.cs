using DunkeyDelivery.Areas.User.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            //var fullName = claimIdentity.Claims.FirstOrDefault(x => x.Type == "FullName");
            //var profilePictureUrl = claimIdentity.Claims.FirstOrDefault(x => x.Type == "ProfilePictureUrl");

            model.SetSharedData(User);

            ViewBag.Title = "Home";
            return View("~/Areas/User/Views/Home/Index.cshtml", model);

        }

        //public async Task<ActionResult> UserLogin()
        //{


        //    var claimIdentity = ((ClaimsIdentity)User.Identity);


        //    UserViewModel1 model = new UserViewModel1();
        //    var response = await ApiCall<UserViewModel1>.CallApi("api/User/GetUserByEmail?Email=", null, false);

        //    if (response is Error)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
        //    }
        //    else
        //    {
        //        model = response.GetValue("Result").ToObject<UserViewModel1>();
        //    }


        //    model.SetSharedData(User);
        //    ViewBag.Title = "Home";
        //    return View("~/Areas/User/Views/Home/Index.cshtml", new HomeViewModel());

        //}

        public ActionResult LogIn()
        {
            ViewBag.Title = "Log In";
            ViewBag.BannerTitle = "LOG IN";
            ViewBag.Path = "Home > Login";
            return View("Login");
        }
        public ActionResult SignUp()
        {
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Sign Up";
            ViewBag.BannerTitle = "Sign Up";
            ViewBag.Path = "Home > Sign Up";
            return View("SignUp");
        }


        public async Task<ActionResult> About()
        {
            AboutUsViewModel returnModel = new AboutUsViewModel();
            var response = await ApiCall<AboutUsViewModel>.CallApi("api/Content/GetContentByType?Type=" + (int)DunkeyDelivery.Content.Types.AboutUs, null, false);

            if (response == null || response is Error)
            {

            }
            else
            {
                var responseResult = response.GetValue("Result").ToObject<AboutUsViewModel>();
                returnModel.VideoUrl=responseResult.VideoUrl;
                returnModel.Description= responseResult.Description;
            }

            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.BannerTitle = "About Us";
            ViewBag.Path = "Home > About";
            returnModel.SetSharedData(User);
            return View(returnModel);
        }

        public async Task<ActionResult> Contact()
        {
            ContactUsViewModel model = new ContactUsViewModel();
            
            var response = await ApiCall<ContactUsViewModel>.CallApi("api/Content/GetContentByType?Type=" + (int)DunkeyDelivery.Content.Types.ContactUs, null, false);

            if (response == null || response is Error)
            {

            }
            else
            {
                var responseResult = response.GetValue("Result").ToObject<ContactUsViewModel>();
                model.Heading = responseResult.Heading;
                model.Description = responseResult.Description;
            }
            
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Contact Us";
            ViewBag.BannerTitle = "Contact Us";
            ViewBag.Path = "Home > Contact Us";
            model.SetSharedData(User);
            return View("ContactUs",model);
        }

        [HttpGet]
        public async Task<ActionResult> SearchByName(string search_string,string CategoryType )
        {
            var response = await ApiCall<Shop>.CallApi("api/Shop/GetSearchedStores?search_string=" + search_string + "&CategoryType=" + CategoryType, null, false);
            var responseResult = response.GetValue("Result").ToObject<Shop>();
            if (CategoryType == "Food") { 
            return PartialView("~/Areas/User/Views/Food/_StoresList.cshtml", responseResult);

            } else
            {
                return PartialView("~/Areas/User/Views/Store/_AllStoreList.cshtml", responseResult);

            }
        }

        [HttpGet]
        public async Task<ActionResult> SearchByAddress(string Type, string Address, int Level/*, double latitude = 0, double longitude = 0*/)
         {
            var response = new JObject();
         


            #region  Setting Banners & Titles 
            if (Type == "Food")
            {
                ViewBag.Title = "Food Stores";
                ViewBag.BannerImage = "banner.jpg";


            }

            else if (Type == "Pharmacy")
            {
                ViewBag.Title = "Pharmacy Stores";
                ViewBag.BannerImage = "pharmacy-banner.png";

            }
            else if (Type == "Laundry")
            {
                ViewBag.Title = "Laundry Stores";
                ViewBag.BannerImage = "alcohol-banner.jpg";
            }
            else if (Type == "Alcohol")
            {
                ViewBag.Title = "Alcohol Stores";
                ViewBag.BannerImage = "alcohol-banner.jpg";
            }
            else if (Type == "Ride")
            {
                ViewBag.Title = "Ride Stores";
                ViewBag.BannerTitle = Type;
                ViewBag.BannerImage = "ridebanner.jpg";
            }
            else if (Type == "Retail")
            {
                ViewBag.Title = "Retail Stores";
                ViewBag.BannerImage = "retails-banner.png";
            }
            else
            {
                ViewBag.Title = "Grocery Stores";
                ViewBag.BannerImage = "grocery-banner.png";
            }
            #endregion

            if (Type == "All")
            {
                response = await ApiCall<Shop>.CallApi("api/Shop/GetAllNearbyStores?Address="+Address, null, false);
            }
            else
            {

                response = await ApiCall<Shop>.CallApi("api/Shop/GetNearbyStores?Type=" + Type + "&Address=" + Address, null, false);
            }
            if (response == null)
            {
                throw new Exception("Some unknown error encountered!");
                return View("Home");
            }
            if(response is Error)
            {
               
                if (Level == 0)
                {
                    var Response = new HomeViewModel { ErrorMessage = (response as Error).ErrorMessage };
                    return View("~/Areas/User/Views/Home/index.cshtml", Response);
                }
                else
                {
                    var Response = new Shop { ErrorMessage = (response as Error).ErrorMessage };
                    if (Type == "Food")
                    {
                        return View("~/Areas/User/Views/Food/SearchFood.cshtml", Response);
                    }
                    else
                    {
                        return View("~/Areas/User/Views/Home/Search.cshtml", Response);
                    }

                }
            }
            else
            {
                var responseResult = response.GetValue("Result").ToObject<Shop>();
                if (Type == "Food")
                {
                    return View("~/Areas/User/Views/Food/SearchFood.cshtml", responseResult);
                }
                else
                {
                    return View("~/Areas/User/Views/Home/Search.cshtml", responseResult);
                }

            }


            #region Comment Section
            //if (response is Error)
            //{

            //    if (Type == "Food" && Level==1)
            //    {
            //        //if (Level == 0)
            //        //{
            //        //    var Response = new HomeViewModel { ErrorMessage = (response as Error).ErrorMessage };
            //        //    return View("~/Areas/User/Views/Home/Index.cshtml", Response);
            //        //}
            //        //else
            //        //{
            //        RedirectToAction("Search","Home",new {Search="Food" });
            //        //var Response = new Shop { ErrorMessage = (response as Error).ErrorMessage };
            //        //return View("~/Areas/User/Views/Food/SearchFood.cshtml", Response);
            //        // }
            //    }
            //    else
            //    {

            //        RedirectToAction("Search", "Home", new { Search = "Grocery" });

            //    }
            //    var invalidResponse = new HomeViewModel { ErrorMessage = (response as Error).ErrorMessage };
            //    return View("~/Areas/User/Views/Home/Index.cshtml", invalidResponse);
            //}


            //var responseResult = response.GetValue("Result").ToObject<Shop>();



            //#region Conditional Banners|Texts|Links
            //if (Type == "Food")
            //{
            //    ViewBag.Title = "Food Stores";
            //    ViewBag.BannerImage = "banner.jpg";
            //    return View("~/Areas/User/Views/Food/SearchFood.cshtml", responseResult);

            //}
            //else if (Type == "Pharmacy")
            //{
            //    ViewBag.Title = "Pharmacy Stores";
            //    ViewBag.BannerImage = "pharmacy-banner.png";

            //}
            //else if (Type == "Laundry")
            //{
            //    ViewBag.Title = "Laundry Stores";
            //    ViewBag.BannerImage = "alcohol-banner.jpg";
            //}
            //else if (Type == "Alcohol")
            //{
            //    ViewBag.Title = "Alcohol Stores";
            //    ViewBag.BannerImage = "alcohol-banner.jpg";
            //}
            //else if (Type == "Ride")
            //{
            //    ViewBag.Title = "Ride Stores";
            //    ViewBag.BannerTitle = Type;
            //    ViewBag.BannerImage = "ridebanner.jpg";
            //}
            //else if (Type == "Retail")
            //{
            //    ViewBag.Title = "Retail Stores";
            //    ViewBag.BannerImage = "retails-banner.jpg";
            //}
            //else if (Type == "All")
            //{
            //    ViewBag.Title = "Food Stores";
            //    ViewBag.BannerImage = "banner.jpg";
            //    return View("~/Areas/User/Views/Food/SearchFood.cshtml", responseResult);
            //}
            //else
            //{
            //    ViewBag.Title = "Grocery Stores";
            //    ViewBag.BannerImage = "grocery-banner.png";
            //}
            //ViewBag.Path = "Home > " + Type + "";

            //ViewBag.BannerTitle = "Restaurants Of " + Type + " ";
            //#endregion


            // return View("~/Areas/User/Views/Home/Search.cshtml", responseResult);
            #endregion
        }

        public async Task<ActionResult> Deals()
        {
            var response = await ApiCall<OfferViewModel>.CallApi("api/Deals/GetOfferPackage", null, false);
            var responseValue = response.GetValue("Result").ToObject<OfferViewModel>();

            //foreach (var offerPkg in responseValue.Offer_Packages.Where(x => !String.IsNullOrEmpty(x.ImageUrl)))
            //    offerPkg.ImageUrl = /*Utility.BaseUrl +*/ offerPkg.ImageUrl;

            //foreach (var offerProduct in responseValue.Offer_Products.Where(x => !String.IsNullOrEmpty(x.ImageUrl)))
            //    offerProduct.ImageUrl = /*Utility.BaseUrl +*/ offerProduct.ImageUrl;

            ViewBag.BannerImage = "dealbanner.jpg";
            ViewBag.Title = " Deals";
            ViewBag.BannerTitle = "DEALS AND PROMOTIONS";
            ViewBag.Path = "Home > Deals / Promotions";
            responseValue.SetSharedData(User);
            return View("Deals", responseValue);
        }

        public ActionResult ForgetPassword()
        {
            ForgetPasswordEmail model = new ForgetPasswordEmail();
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = " Forgot Password";
            ViewBag.BannerTitle = "Forgot Password";
            ViewBag.Path = "Home > Forgot Password";
            model.SetSharedData(User);
            return View("ForgetPassword",model);
        }

        public ActionResult ResetPassword(string code = "")
        {
            NewPassword model = new NewPassword();
            model.SetSharedData(User);
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = " Forget Password";
            ViewBag.BannerTitle = "New Password";
            ViewBag.Path = "Home > New Password";
            ViewBag.Code = code;
            return View("NewPassword",model);
        }
        public async Task<ActionResult> ResetPasswordSubmit(string code, NewPassword model)
        {
            model.Code = code;
            NewPassword passModel =new NewPassword();

            var response = await ApiCall<NewPassword>.CallApi("api/User/ResetPassword", model);
            if (response is Error)
            {
                passModel.ErrorMessage=(response as Error).ErrorMessage;
                return View("~/Areas/User/Views/Home/NewPassword.cshtml", passModel);
                //  return Json(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage, JsonRequestBehavior.AllowGet);
            }
            return new ContentResult();
            //return View("~/Areas/User/Views/Home/NewPassword.cshtml", new NewPassword()); ;
            //return Json(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage, JsonRequestBehavior.AllowGet);
            //return View("~/Areas/User/Views/Home/Index.cshtml", passModel);

        }



        public async Task<ActionResult> NewPassword(ForgetPasswordEmail model)
        {

            try
            {
                
                   var response = await ApiCall<ForgetPasswordEmail>.CallApi("api/User/ResetPasswordThroughEmail?email=" + model.Email + "", null, false);
                   //var response = await ApiCall<ForgetPasswordEmail>.CallApi("api/User/PasswordResetThroughEmail?email=" + model.Email + "", null, false);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

                }

                var responseValue = response.GetValue("Result").ToObject<ForgetPasswordEmail>();
                //var callbackUrl = Url.Link("Default", new { Controller = "ResetPassword", Action = "ResetPassword", code = responseValue.Code });
                //var callbackUrl = Url.Action("ResetPassword", "Home", new { area = "User", code = responseValue.forgetPasswordToken.Last().Code });
                var callbackUrl = Utility.EmailBaseUrl + "User/Home/ResetPassword?code=" + responseValue.forgetPasswordToken.Last().Code;
                const string subject = "Your Password Has Been Changed";
                string body = "You recently requested to change password of "+model.Email+" "+ Environment.NewLine + " To Complete your request, please click link below: "+Environment.NewLine+" " +callbackUrl;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(EmailUtil.FromMailAddress.Address, EmailUtil.FromPassword)
                };

                var message = new MailMessage(EmailUtil.FromMailAddress, new MailAddress(model.Email))
                {
                    Subject = subject,
                    Body = body
                };

                smtp.Send(message);



                ViewBag.BannerImage = "press-top-banner.jpg";
                ViewBag.Title = " New Password";
                ViewBag.BannerTitle = "Forgot Password";
                ViewBag.Path = "Home > Forgot Password";
                return View("~/Areas/User/Views/Home/ForgetPassword.cshtml", responseValue);
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                throw;
            }
        }

        public async Task<ActionResult> PaggingByType(SearchViewModel model)
        {


            var response = await ApiCall<Shop>.CallApi("api/Shop/SearchByType?Type=" + model.Search + "&items=" + model.ObjectPerPAge + "&Page=" + model.PageNumber, null, false);
            var responseValue = response.GetValue("Result").ToObject<Shop>();

            if (responseValue.Store.Count() != 0)
            {


                #region SettingDefaultStoreImage
                foreach (var product in responseValue.Store)
                {
                    if (product.ImageUrl == null)
                    {

                        product.ImageUrl = DefaultImages.StoreDefaultImage();
                    }
                }
                #endregion

            }

            if (model.Search == "Food")
            {
                return PartialView("~/Areas/User/Views/Food/_StoresList.cshtml", responseValue);
            }
            else
            {
                return PartialView("~/Areas/User/Views/Store/_AllStoreList.cshtml", responseValue);
            }


        }

        public async Task<ActionResult> Search(SearchViewModel model)
        {
           
            if (model.Search != "Food")
            {
                model.ObjectPerPAge = 4;
                model.PageNumber = 0;
            }
            var response = await ApiCall<Shop>.CallApi("api/Shop/SearchByType?Type=" + model.Search + "&items="+model.ObjectPerPAge+"&Page="+model.PageNumber, null, false);
            var responseValue = response.GetValue("Result").ToObject<Shop>();
            if (responseValue.Store.Count() != 0)
            {


                #region SettingDefaultStoreImage
                foreach (var product in responseValue.Store)
                {
                    if (product.ImageUrl == null)
                    {

                        product.ImageUrl = DefaultImages.StoreDefaultImage();
                    }
                }
                #endregion

            }
            #region Conditional Banners|Texts|Links
            if (model.Search == "Food")
            {
                ViewBag.Title = "Food";
                ViewBag.BannerImage = "banner.jpg";
                responseValue.SetSharedData(User);
                return View("~/Areas/User/Views/Food/SearchFood.cshtml", responseValue);

            }
            else if (model.Search == "Pharmacy")
            {
                ViewBag.Title = "Pharmacy";
                ViewBag.BannerImage = "pharmacy-banner.png";

            }
            else if (model.Search == "Laundry")
            {
                ViewBag.Title = "Laundry";
                ViewBag.BannerImage = "laundry.png";
            }
            else if (model.Search == "Alcohol")
            {
                ViewBag.Title = "Alcohol";
                ViewBag.BannerImage = "alcohol-banner.png";
            }
            else if (model.Search == "Ride")
            {
                ViewBag.Title = "Ride";
                ViewBag.BannerTitle = model.Search;
                ViewBag.BannerImage = "ridebanner.png";
            }
            else if (model.Search == "Retail")
            {
                ViewBag.Title = "Retail";
                ViewBag.BannerImage = "retails-banner.png";
            }
            else
            {
                ViewBag.Title = "Grocery";
                ViewBag.BannerImage = "grocery-banner.png";
            }
            ViewBag.Path = "Home > " + model.Search + "";

            ViewBag.BannerTitle = "Restaurants Of " + model.Search + " ";
            #endregion


            responseValue.SetSharedData(User);
            return View("~/Areas/User/Views/Home/Search.cshtml", responseValue);

        }

        public async Task<JsonResult> Subscribe(HomeViewModel model)
        {
            var response = await ApiCall<HomeViewModel>.CallApi("api/User/SendAppLink", model);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SubmitContactUs(ContactUsViewModel model)
        {

            var response = await ApiCall<ContactUsViewModel>.CallApi("api/User/SubmitContactUs", model, true);
            var responseResult = response.GetValue("Result").ToObject<string>();
            return Json(responseResult, JsonRequestBehavior.AllowGet);
          
        }










    }
}