using DunkeyDelivery.Areas.User.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
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

        public async Task<ActionResult> PageView(int id, int? PageSize, int? PageNo)
        {
            if (id == 0)
            {
                #region OrderHistory
                // for Order History 
                if (!PageNo.HasValue)
                {
                    PageNo = 0;
                }
                PageSize = 3;
                var claimIdentity = ((ClaimsIdentity)User.Identity);
                var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
                var response = await ApiCall<OrdersHistoryViewModel>.CallApi("api/Order/GetOrdersHistory?UserId=" + userId + "&SignInType=" + 0 + "&IsCurrentOrder=" + true + "&PageSize=" + PageSize + "&PageNo=" + PageNo, null, false);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

                }
                if (response == null)
                {

                }


                var responseResult = response.GetValue("Result").ToObject<OrdersHistoryViewModel>();
                responseResult.SetSharedData(User);
                return PartialView("_OrderHistory", responseResult);
                #endregion

            }
            else if (id == 1)
            {
                #region RecurringOrders
                // for recurring orders
                if (!PageNo.HasValue)
                {
                    PageNo = 0;
                }
                PageSize = 3;
                var claimIdentity = ((ClaimsIdentity)User.Identity);
                var userId = claimIdentity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
                var response = await ApiCall<OrdersHistoryViewModel>.CallApi("api/Order/GetOrdersHistory?UserId=" + userId + "&SignInType=" + 0 + "&IsCurrentOrder=" + true + "&PageSize=" + PageSize + "&PageNo=" + PageNo, null, false);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

                }
                if (response == null)
                {

                }


                var responseResult = response.GetValue("Result").ToObject<OrdersHistoryViewModel>();
                responseResult.SetSharedData(User);

                return PartialView("_RecurringOrders", responseResult);
                #endregion
            }
            else if (id == 2)
            {
                #region ProfileView
                // for account ( Profile View )
                Global.sharedDataModel.SetSharedData(User);
                ProfileViewModel model = new ProfileViewModel(Global.sharedDataModel);

                //Global.sharedDataModel.SetSharedData(User);
                return PartialView("_Account", model);
                #endregion
            }
            else if (id == 3)
            {
                #region Address
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
                return PartialView("_Addresses", responseResult);
                #endregion
            }
            else if (id == 4)
            {
                #region CreditCard
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
                #endregion

            }
            else if (id == 5)
            {
                #region AddAddress
                return PartialView("_AddAddress", new AddressViewModel());
                #endregion
            }
            else if (id == 6)
            {
                #region AddCreditCard
                return PartialView("_AddCreditCard", new CreditCardViewModel());
                #endregion
            }
            Global.sharedDataModel.SetSharedData(User);
            return PartialView("_Account", Global.sharedDataModel);
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

            var UpdatedUser = ChangeBaseViewModel(model);
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
            model.User_ID = Convert.ToInt32(userId);



            var response = await ApiCall<AddressViewModel>.CallApi("api/User/AddAddress", model);


            if (response is Error)
            {
                //return new HttpStatusCodeResult((response as Error).ErrorMessage);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "**basra*"+(response as Error).ErrorMessage+ "*basra**");
                //return Json((response as Error).ErrorMessage, JsonRequestBehavior.AllowGet);
                //return Content("{\"ErrorMessage\":\"" + (response as Error).ErrorMessage + "\"}");
                //return new HttpStatusCodeResult(404, "Can't find that");
            }
            if (response == null)
            {

            }


            return Json(response,JsonRequestBehavior.AllowGet);
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
            var response = await ApiCall<string>.CallApi("api/User/RemoveAddress?address_id=" + id + "&User_Id=" + User_ID, null, false);



            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }
            if (response == null)
            {

            }
            var responseShopValue = response.GetValue("Result").ToObject<string>();
            return RedirectToAction("PageView", new { id = 3 });
            //  return Json(responseShopValue, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> RepeatOrder(string orderId)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var response = await ApiCall<OrdersHistoryViewModel>.CallApi("api/Order/RepeatOrder?OrderId=" + orderId, null, false);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

                }
                if (response == null)
                {

                }


                var responseResult = response.GetValue("Result").ToObject<OrdersHistoryViewModel>();


                return Json(responseResult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<ActionResult> ShoppingCart()
        {
            try
            {
                ShoppingCartViewModel model = new ShoppingCartViewModel();
                model.SetSharedData(User);
                ViewBag.Title = "Shopping Cart";
                ViewBag.BannerImage = "press-top-banner.jpg";
                ViewBag.BannerTitle = "SHOPPING CART";
                ViewBag.Path = "Home > Shopping Cart";
                return View("~/Areas/User/Views/ShoppingCart/ShoppingCart.cshtml", model);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //        var TotalCartItems = 0;
        //        HttpCookie cookie = new HttpCookie("Cart");
        //        ShoppingCartViewModel model = new ShoppingCartViewModel();
        //        Cart cart = new Cart();
        //                if (Request.RequestContext.HttpContext.Request.Cookies.AllKeys.Contains("Cart"))
        //                {
        //                    var cartCookie = Request.RequestContext.HttpContext.Request.Cookies.Get("Cart");
        //        cart = JObject.Parse(cartCookie.Value.Replace("%0d%0a", "")).ToObject<Cart>();

        //                    foreach (var store in cart.Stores)
        //                    {
        //                        TotalCartItems += store.CartItems.Count;
        //                    }
        //    cart.TotalCartItems = TotalCartItems;
        //                }
        //model.SetSharedData(User);
        //                model.cart = cart;
        //                ViewBag.Title = "Shopping Cart";
        //                ViewBag.BannerImage = "press-top-banner.jpg";
        //                ViewBag.BannerTitle = "SHOPPING CART";
        //                ViewBag.Path = "Home > Shopping Cart";
        //                return PartialView("~/Areas/User/Views/ShoppingCart/_ShoppingCart.cshtml", model);

        public async Task<ActionResult> GetShoppingCart()
        {
            try
            {
                var TotalCartItems = 0;
                HttpCookie cookie = new HttpCookie("Cart");
                ShoppingCartViewModel model = new ShoppingCartViewModel();
                Cart cart = new Cart();
                if (Request.RequestContext.HttpContext.Request.Cookies.AllKeys.Contains("Cart"))
                {
                    var cartCookie = Request.RequestContext.HttpContext.Request.Cookies.Get("Cart");
                    cart = JObject.Parse(cartCookie.Value.Replace("%0d%0a", "")).ToObject<Cart>();

                    foreach (var store in cart.Stores)
                    {
                        TotalCartItems += store.CartItems.Count;
                    }
                    cart.TotalCartItems = TotalCartItems;
                }
                model.SetSharedData(User);
                model.cart = cart;
                ViewBag.Title = "Shopping Cart";
                ViewBag.BannerImage = "press-top-banner.jpg";
                ViewBag.BannerTitle = "SHOPPING CART";
                ViewBag.Path = "Home > Shopping Cart";
                return PartialView("~/Areas/User/Views/ShoppingCart/_ShoppingCart.cshtml", model);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}