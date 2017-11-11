using DunkeyDelivery.Areas.User.Models;
using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static DunkeyDelivery.Utility;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class OrderController : Controller
    {
        // GET: Dashboard/Order
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OrderItems(float? MinOrder)
        {
            Cart cart = new Cart();

            var cartCookie = Request.RequestContext.HttpContext.Request.Cookies.Get("Cart");
            if (cartCookie != null && String.IsNullOrEmpty(cartCookie.Value) == false)
            {
                //Below two lines will forcefully empty the cookie
                //cartCookie.Expires = DateTime.Now.AddDays(-1);
                //Request.RequestContext.HttpContext.Response.Cookies.Add(cartCookie);
                cart = JObject.Parse(cartCookie.Value.Replace("%0d%0a", "")).ToObject<Cart>();
                //cart.CartItems.Add(cartItem);
            }
            if (MinOrder != null)
            {
                cart.MinOrder = MinOrder;
            }
            return PartialView("_OrderItems", cart);
        }
        public ActionResult OrderItemsDeals(float? MinOrder)
        {
            Cart cart = new Cart();

            var cartCookie = Request.RequestContext.HttpContext.Request.Cookies.Get("Cart");
            if (cartCookie != null && String.IsNullOrEmpty(cartCookie.Value) == false)
            {
                //Below two lines will forcefully empty the cookie
                //cartCookie.Expires = DateTime.Now.AddDays(-1);
                //Request.RequestContext.HttpContext.Response.Cookies.Add(cartCookie);
                cart = JObject.Parse(cartCookie.Value.Replace("%0d%0a", "")).ToObject<Cart>();
                //cart.CartItems.Add(cartItem);
            }
            if (MinOrder != null)
            {
                cart.MinOrder = MinOrder;
            }
            return PartialView("_OrderItemsDeals", cart);
        }
        public ActionResult DeliveryDetails()
        {
            var stripePublishKey = ConfigurationManager.AppSettings["StripePublishableKey"];
            ViewBag.StripePublishKey = stripePublishKey;
            DeliveryDetailsViewModel deliveryModel = new DeliveryDetailsViewModel();
            deliveryModel.Cart = GetCartData();
            if (deliveryModel.Cart.Stores.Count == 0)
            {
                return RedirectToAction("Index", "Home", new { area = "User" });
            }
            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = "Delivery Details";
            ViewBag.BannerTitle = "Delivery Details";
            ViewBag.Path = "Home > Delivery Details";
            deliveryModel.SetSharedData(User);
            return View("DeliveryDetails", deliveryModel);
        }


        // reusable function to get latest cookies data 
        public Cart GetCartData()
        {
            var TotalCartItems = 0;
            HttpCookie cookie = new HttpCookie("Cart");
            Cart cart = new Cart();
            if (Request.RequestContext.HttpContext.Request.Cookies.AllKeys.Contains("Cart"))
            {
                var cartCookie = Request.RequestContext.HttpContext.Request.Cookies.Get("Cart");
                cart = JObject.Parse(cartCookie.Value.Replace("%0d%0a", "")).ToObject<Cart>();
                //CartItem existingCartItem;
                foreach (var store in cart.Stores)
                {
                    TotalCartItems += store.CartItems.Count;
                }
                cart.TotalCartItems = TotalCartItems;
            }
            cart.Tax = cart.Stores.Distinct(new StoreItem.DistinctComparerOnBusinessType()).Sum(x => x.BusinessTypeTax);

            return cart;
        }

        public ActionResult CartWithData()
        {
            Cart cart = new Cart();
            cart = GetCartData();
            return PartialView("_Cart", cart);
        }

        public ActionResult Cart()
        {
            return PartialView("_Cart");
        }
        public ActionResult DeliveryTime()
        {
            return PartialView("_DeliveryTime");
        }

        public async Task<ActionResult> OrderSummary(DeliveryDetailsViewModel model, string stripeEmail, string stripeToken)
        {
            try
            {
                if (model.DeliveryDetails == null)
                {
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }

                Cart cart = new Models.Cart();
                cart = GetCartData();
                model.Cart = cart;
                OrderViewModel orderModel = new OrderViewModel();
                orderModel.AdditionalNote = model.DeliveryDetails.AdditionalNote;
                orderModel.DeliveryAddress = model.DeliveryDetails.Address;
                orderModel.TipAmount = model.TipAmount;
                orderModel.DeliveryDetails = model.DeliveryDetails;
                model.SetSharedData(User);
                //orderModel.PaymentMethodType = model.PaymentInformation.PaymentType;
                if (!string.IsNullOrEmpty(model.Id))
                {
                    orderModel.UserId = Convert.ToInt32(model.Id);
                }

                foreach (var store in cart.Stores)
                {
                    foreach (var cartItem in store.CartItems)
                    {
                        orderModel.Cart.CartItems.Add(new CartItemViewModel { ItemId = cartItem.ItemId, StoreId = cartItem.StoreId, ItemType = cartItem.Type, Qty = cartItem.Qty });
                    }
                }

                orderModel.StripeAccessToken = model.StripeId;
                orderModel.StripeEmail = model.StripeEmail;

                //Charge user
                //StripeCharge stripeCharge = Utility.GetStripeChargeInfo(model.StripeEmail, model.StripeId, Convert.ToInt32(model.Cart.Total + model.TipAmount));

                //if (stripeCharge.Status != "succeeded")
                //{
                //    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Payment Failed");
                //}

                var responseOrder = await ApiCall<OrderViewModel>.CallApi("api/Order/InsertOrder", orderModel);
                if (responseOrder == null || responseOrder is Error)
                {
                    TempData["ErrorMessage"] = (responseOrder as Error).ErrorMessage;
                    return RedirectToAction("DeliveryDetails");
                }

                var orderServer = responseOrder.GetValue("Result").ToObject<OrderBindingModel>();

                // Remove cart from Cookies
                if (Request.Cookies["Cart"] != null)
                {
                    var c = new HttpCookie("Cart");
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }
                orderServer.SetSharedData(User);
                ViewBag.BannerImage = "press-top-banner.jpg";
                ViewBag.Title = "Order Summary";
                ViewBag.BannerTitle = "Order Summary";
                ViewBag.Path = "Home > Order Summary";
                return View("OrderSummary", orderServer);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "An error occured while processing your request.");
            }
        }

        public JsonResult StoreToSession()
        {
            return Json(JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public JsonResult GetCart()
        {
            var TotalCartItems = 0;
            HttpCookie cookie = new HttpCookie("Cart");
            Cart cart = new Cart();
            if (Request.RequestContext.HttpContext.Request.Cookies.AllKeys.Contains("Cart"))
            {
                var cartCookie = Request.RequestContext.HttpContext.Request.Cookies.Get("Cart");
                cart = JObject.Parse(cartCookie.Value.Replace("%0d%0a", "")).ToObject<Cart>();
                //CartItem existingCartItem;
                foreach (var store in cart.Stores)
                {
                    TotalCartItems += store.CartItems.Count;
                }
                //var existingCartItem = cart.CartItems.FirstOrDefault(x => x.ItemId == model.ItemId && x.Type == model.Type && x.StoreId == model.StoreId);
                cart.TotalCartItems = TotalCartItems;
                //var existingStore = cart.Stores;
            }
            return Json(cart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddToCart(CartItem model)
        {
            HttpCookie cookie = new HttpCookie("Cart");
            Cart cart = new Cart();
            if (Request.RequestContext.HttpContext.Request.Cookies.AllKeys.Contains("Cart"))
            {
                var cartCookie = Request.RequestContext.HttpContext.Request.Cookies.Get("Cart");
                cart = JObject.Parse(cartCookie.Value.Replace("%0d%0a", "")).ToObject<Cart>();
                //CartItem existingCartItem;
                //var existingCartItem = cart.CartItems.FirstOrDefault(x => x.ItemId == model.ItemId && x.Type == model.Type && x.StoreId == model.StoreId);
                var existingStore = cart.Stores.FirstOrDefault(x => x.StoreId == model.StoreId);
                if (existingStore != null)
                {
                    var existingCartItem = existingStore.CartItems.FirstOrDefault(x => x.ItemId == model.ItemId && x.Type == model.Type);
                    
                    if (existingCartItem != null)
                    {
                        existingCartItem.Qty += 1;
                        existingCartItem.Total = existingCartItem.Price * existingCartItem.Qty;
                    }
                    else
                    {
                        model.Total = model.Price;
                        existingStore.CartItems.Add(model);
                    }
                }
                else
                {
                    model.Total = model.Price;
                    cart.Stores.Add(new StoreItem { BusinessTypeTax = model.BusinessTypeTax, BusinessType = model.BusinessType, StoreId = model.StoreId, StoreName = model.StoreName, CartItems = new List<CartItem> { model } });
                }

            }
            else
            {
                model.Total = model.Price;
                cart.Stores.Add(new StoreItem { BusinessTypeTax = model.BusinessTypeTax, BusinessType = model.BusinessType, StoreId = model.StoreId, StoreName = model.StoreName, CartItems = new List<CartItem> { model } });
            }

            //cart.Total = cart.CartItems.Sum(x => x.Total);
            cart.Tax = cart.Stores.Distinct(new StoreItem.DistinctComparerOnBusinessType()).Sum(x => x.BusinessTypeTax);
            cart.Total = cart.Stores.SelectMany(x => x.CartItems).Sum(x => x.Total) + cart.Tax;
            cookie.Value = JObject.FromObject(cart).ToString();
            Request.RequestContext.HttpContext.Response.Cookies.Add(cookie);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeleteCartItem(CartItem model)
        {
            Cart cart;
            if (Request.RequestContext.HttpContext.Request.Cookies.AllKeys.Contains("Cart"))
            {
                var cartCookie = Request.RequestContext.HttpContext.Request.Cookies.Get("Cart");
                cart = JObject.Parse(cartCookie.Value.Replace("%0d%0a", "")).ToObject<Cart>();

                //var cartItem = cart.CartItems.FirstOrDefault(x => x.ItemId == model.ItemId && x.Type == model.Type && x.StoreId == model.StoreId);
                var existingStore = cart.Stores.FirstOrDefault(x => x.StoreId == model.StoreId);
                var cartItem = existingStore.CartItems.FirstOrDefault(x => x.ItemId == model.ItemId && x.Type == model.Type);

                if (cartItem != null)
                {
                    if (cartItem.Qty > 1)
                    {
                        cartItem.Qty -= 1;
                        cartItem.Total = cartItem.Price * cartItem.Qty;
                    }
                    else
                    {

                        existingStore.CartItems.Remove(cartItem);

                        if (existingStore.CartItems.Count == 0)
                            cart.Stores.Remove(existingStore);
                    }

                }

                //cart.Total = cart.CartItems.Sum(x => x.Total);
                cart.Tax = cart.Stores.Distinct(new StoreItem.DistinctComparerOnBusinessType()).Sum(x => x.BusinessTypeTax);
                cart.Total = cart.Stores.SelectMany(x => x.CartItems).Sum(x => x.Total) + cart.Tax;
                cartCookie.Value = JObject.FromObject(cart).ToString();
                Request.RequestContext.HttpContext.Response.Cookies.Set(cartCookie);
            }

            return Content("Ok");
        }

        [HttpPost]
        public ActionResult RemoveCartItem(CartItem model)
        {
            Cart cart;
            if (Request.RequestContext.HttpContext.Request.Cookies.AllKeys.Contains("Cart"))
            {
                var cartCookie = Request.RequestContext.HttpContext.Request.Cookies.Get("Cart");
                cart = JObject.Parse(cartCookie.Value.Replace("%0d%0a", "")).ToObject<Cart>();

                //var cartItem = cart.CartItems.FirstOrDefault(x => x.ItemId == model.ItemId && x.Type == model.Type && x.StoreId == model.StoreId);
                var existingStore = cart.Stores.FirstOrDefault(x => x.StoreId == model.StoreId);
                var cartItem = existingStore.CartItems.FirstOrDefault(x => x.ItemId == model.ItemId && x.Type == model.Type);

                if (cartItem != null)
                {
                    existingStore.CartItems.Remove(cartItem);
                    if (existingStore.CartItems.Count == 0)
                        cart.Stores.Remove(existingStore);


                }

                //cart.Total = cart.CartItems.Sum(x => x.Total);
                cart.Tax = cart.Stores.Distinct(new StoreItem.DistinctComparerOnBusinessType()).Sum(x => x.BusinessTypeTax);
                cart.Total = cart.Stores.SelectMany(x => x.CartItems).Sum(x => x.Total) + cart.Tax;
                cartCookie.Value = JObject.FromObject(cart).ToString();
                Request.RequestContext.HttpContext.Response.Cookies.Set(cartCookie);
            }

            return Content("Ok");
        }
    }
}