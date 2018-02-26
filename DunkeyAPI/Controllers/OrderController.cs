using DAL;
using DunkeyAPI.ExtensionMethods;
using DunkeyAPI.Utility;
using DunkeyAPI.ViewModels;
using DunkeyDelivery.CustomAuthorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DunkeyAPI.Utility;
using System.Data.Entity;
using static DunkeyAPI.Utility.Global;
using Stripe;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        [HttpPost]
        [Route("InsertOrder")]
        public async Task<IHttpActionResult> InsertOrder(OrderViewModel model)
        {
            try
            {
                Order order;
                if (System.Web.HttpContext.Current.Request.Params["Cart"] != null)
                    model.Cart = JsonConvert.DeserializeObject<CartViewModel>(System.Web.HttpContext.Current.Request.Params["Cart"]);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (model.Cart.CartItems.Count() > 0)
                {
                    order = new Order();
                    using (DunkeyContext ctx = new DunkeyContext())
                    {
                        order.MakeOrder(model, ctx,0); // 0 for website insert order

                        order.DeliveryTime_From = DateTime.Now;
                        order.DeliveryTime_To = DateTime.Now;

                        //Charge User
                        // StripeCharge stripeCharge = DunkeyDelivery.Utility.GetStripeChargeInfo(model.StripeEmail, model.StripeAccessToken, Convert.ToInt32(order.Total));

                        //if (stripeCharge.Status != "succeeded")
                        //{
                        //    return Ok(new CustomResponse<Error> { Message = "Payment Failed", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "We are unable to process your payments. Please try sometime later" } });
                        //}

                        ctx.Orders.Add(order);
                        await ctx.SaveChangesAsync();
                        var CurrentUser = ctx.Users.Where(x => x.Id == model.UserId).FirstOrDefault();
                        if (CurrentUser.RewardPoints == 0)
                        {
                            CurrentUser.RewardPoints = order.Subtotal * DunkeyDelivery.Global.PointsToReward;
                        }
                        else
                        {
                            CurrentUser.RewardPoints = CurrentUser.RewardPoints + (order.Subtotal * DunkeyDelivery.Global.PointsToReward);
                        }
                        ctx.SaveChanges();

                        return Ok(new CustomResponse<Order> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = order });
                    }
                }
                else
                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "No items in the cart." } });

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }




        [HttpPost]
        [Route("InsertOrderMobile")]
        public async Task<IHttpActionResult> InsertOrderMobile(OrderViewModel model)
        {
            try
            {
                Order order;
                if (System.Web.HttpContext.Current.Request.Params["Cart"] != null)
                    model.Cart = JsonConvert.DeserializeObject<CartViewModel>(System.Web.HttpContext.Current.Request.Params["Cart"]);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (model.Cart.CartItems.Count() > 0)
                {
                    order = new Order();
                    using (DunkeyContext ctx = new DunkeyContext())
                    {
                        order.MakeOrder(model, ctx,1); // 1 for mobile insert order
                        order.DeliveryDetails_AddtionalNote = model.AdditionalNote;
                        order.PaymentMethod = model.PaymentMethodType;
                        order.Frequency = model.Frequency;
                        //order.DeliveryTime_From = DateTime.Now;
                        //order.DeliveryTime_To = DateTime.Now;

                        //Charge User
                        // StripeCharge stripeCharge = DunkeyDelivery.Utility.GetStripeChargeInfo(model.StripeEmail, model.StripeAccessToken, Convert.ToInt32(order.Total));

                        //if (stripeCharge.Status != "succeeded")
                        //{
                        //    return Ok(new CustomResponse<Error> { Message = "Payment Failed", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "We are unable to process your payments. Please try sometime later" } });
                        //}

                        ctx.Orders.Add(order);
                        await ctx.SaveChangesAsync();
                        var CurrentUser = ctx.Users.Where(x => x.Id == model.UserId).FirstOrDefault();
                        if (CurrentUser.RewardPoints == 0)
                        {
                            CurrentUser.RewardPoints = order.Subtotal * DunkeyDelivery.Global.PointsToReward;
                        }
                        else
                        {
                            CurrentUser.RewardPoints = CurrentUser.RewardPoints + (order.Subtotal * DunkeyDelivery.Global.PointsToReward);
                        }
                        ctx.SaveChanges();

                        return Ok(new CustomResponse<Order> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = order });
                    }
                }
                else
                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "No items in the cart." } });

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetOrdersHistory")]
        public async Task<IHttpActionResult> GetOrdersHistory(int UserId, int SignInType, bool IsCurrentOrder, int PageSize, int PageNo)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    OrdersHistoryViewModel orderHistory = new OrdersHistoryViewModel();

                    if (SignInType == (int)RolesCode.User)
                    {
                        if (IsCurrentOrder)
                            orderHistory.Count = ctx.Orders.Count(x => x.User_ID == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed);
                        else
                            orderHistory.Count = ctx.Orders.Count(x => x.User_ID == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed);
                    }
                    else
                    {
                        if (IsCurrentOrder)
                            orderHistory.Count = ctx.Orders.Count(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed);
                        else
                            orderHistory.Count = ctx.Orders.Count(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed);
                    }

                    if (orderHistory.Count == 0)
                    {
                        return Ok(new CustomResponse<OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = orderHistory });
                    }

                    #region OrderQuery
                    string orderQuery = String.Empty;
                    if (SignInType == (int)RolesCode.User)
                    {
                        orderQuery = @"SELECT *, Users.FullName as UserFullName FROM Orders join Users on Users.ID = Orders.User_ID where Orders.User_Id = " + UserId + @" and Orders.IsDeleted = 0  ORDER BY Orders.id DESC OFFSET " + PageNo * PageSize + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY;";
                    }
                    else
                    {
                        orderQuery = @"SELECT *, Users.FullName as UserFullName FROM Orders 
						join Users on Users.ID = Orders.User_ID
						where Orders.DeliveryMan_Id = " + UserId + @" and Orders.IsDeleted = 0  
						ORDER BY Orders.id DESC OFFSET " + PageNo * PageSize + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY;";
                    }

                    #endregion

                    orderHistory.orders = ctx.Database.SqlQuery<OrderVM>(orderQuery).ToList();

                    var orderIds = string.Join(",", orderHistory.orders.Select(x => x.Id.ToString()));

                    #region StoreOrderQuery
                    var storeOrderQuery = @"
						select
						StoreOrders.*,
						Stores.BusinessName as StoreName,
						Stores.ImageUrl from StoreOrders 
						join Stores on Stores.Id = StoreOrders.Store_Id
						where 
						Order_Id in (" + orderIds + @")
						";
                    #endregion

                    var storeOrders = ctx.Database.SqlQuery<StoreOrderViewModel>(storeOrderQuery).ToList();

                    var storeOrderIds = string.Join(",", storeOrders.Select(x => x.Id.ToString()));

                    #region OrderItemsQuery

                    var orderItemsQuery = @"SELECT
  CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Id
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Id
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.Id
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.Id
  END AS ItemId,
  Order_Items.Name AS Name,
  Order_Items.Price AS Price,
  CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Image
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.ImageUrl
  END AS ImageUrl,
  Order_Items.Id,
  Order_Items.Qty,

  Order_Items.StoreOrder_Id
FROM Order_Items
LEFT JOIN products
  ON products.Id = Order_Items.Product_Id
LEFT JOIN Packages
  ON Packages.Id = Order_Items.Package_Id
LEFT JOIN Offer_Products
  ON Offer_Products.Id = Order_Items.Offer_Product_Id
LEFT JOIN Offer_Packages
  ON Offer_Packages.Id = Order_Items.Offer_Package_Id
WHERE StoreOrder_Id IN (" + storeOrderIds + ")";
                    #endregion

                    var orderItems = ctx.Database.SqlQuery<OrderItemViewModel>(orderItemsQuery).ToList();

                    foreach (var orderItem in orderItems)
                    {
                        storeOrders.FirstOrDefault(x => x.Id == orderItem.StoreOrder_Id).OrderItems.Add(orderItem);
                    }

                    foreach (var storeOrder in storeOrders)
                    {
                        orderHistory.orders.FirstOrDefault(x => x.Id == storeOrder.Order_Id).StoreOrders.Add(storeOrder);
                    }

                    return Ok(new CustomResponse<OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = orderHistory });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetOrdersHistoryMobile")]
        public async Task<IHttpActionResult> GetOrdersHistoryMobile(int UserId, int SignInType, bool IsCurrentOrder, int PageSize, int PageNo)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    OrdersHistoryViewModel orderHistory = new OrdersHistoryViewModel();

                    if (SignInType == (int)RolesCode.User)
                    {
                        if (IsCurrentOrder)
                            orderHistory.Count = ctx.Orders.Count(x => x.User_ID == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed);
                        else
                            orderHistory.Count = ctx.Orders.Count(x => x.User_ID == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed);
                    }
                    else
                    {
                        if (IsCurrentOrder)
                            orderHistory.Count = ctx.Orders.Count(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed);
                        else
                            orderHistory.Count = ctx.Orders.Count(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed);
                    }

                    if (orderHistory.Count == 0)
                    {
                        return Ok(new CustomResponse<OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = orderHistory });
                    }

                    #region OrderQuery
                    string orderQuery = String.Empty;
                    if (SignInType == (int)RolesCode.User)
                    {
                        orderQuery = @"SELECT *, Users.FullName as UserFullName FROM Orders join Users on Users.ID = Orders.User_ID where Orders.User_Id = " + UserId + @" and Orders.IsDeleted = 0  ORDER BY Orders.id DESC OFFSET " + PageNo * PageSize + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY;";
                    }
                    else
                    {
                        orderQuery = @"SELECT *, Users.FullName as UserFullName FROM Orders 
						join Users on Users.ID = Orders.User_ID
						where Orders.DeliveryMan_Id = " + UserId + @" and Orders.IsDeleted = 0  
						ORDER BY Orders.id DESC OFFSET " + PageNo * PageSize + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY;";
                    }

                    #endregion

                    orderHistory.orders = ctx.Database.SqlQuery<OrderVM>(orderQuery).ToList();

                    var orderIds = string.Join(",", orderHistory.orders.Select(x => x.Id.ToString()));

                    #region StoreOrderQuery
                    var storeOrderQuery = @"
						select
						StoreOrders.*,
						Stores.BusinessName as StoreName,
						Stores.ImageUrl from StoreOrders 
						join Stores on Stores.Id = StoreOrders.Store_Id
						where 
						Order_Id in (" + orderIds + @")
						";
                    #endregion

                    var storeOrders = ctx.Database.SqlQuery<StoreOrderViewModel>(storeOrderQuery).ToList();

                    var storeOrderIds = string.Join(",", storeOrders.Select(x => x.Id.ToString()));

                    #region OrderItemsQuery

                    var orderItemsQuery = @"SELECT
  CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Id
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Id
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.Id
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.Id
  END AS ItemId,
  Order_Items.Name AS Name,
  Order_Items.Price AS Price,
  CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Image
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.ImageUrl
  END AS ImageUrl,
  Order_Items.Id,
  Order_Items.Qty,

  Order_Items.StoreOrder_Id
FROM Order_Items
LEFT JOIN products
  ON products.Id = Order_Items.Product_Id
LEFT JOIN Packages
  ON Packages.Id = Order_Items.Package_Id
LEFT JOIN Offer_Products
  ON Offer_Products.Id = Order_Items.Offer_Product_Id
LEFT JOIN Offer_Packages
  ON Offer_Packages.Id = Order_Items.Offer_Package_Id
WHERE StoreOrder_Id IN (" + storeOrderIds + ")";
                    #endregion

                    var orderItems = ctx.Database.SqlQuery<OrderItemViewModel>(orderItemsQuery).ToList();

                    foreach (var orderItem in orderItems)
                    {
                        storeOrders.FirstOrDefault(x => x.Id == orderItem.StoreOrder_Id).OrderItems.Add(orderItem);

                    }

                    foreach (var storeOrder in storeOrders)
                    {
                        orderHistory.orders.FirstOrDefault(x => x.Id == storeOrder.Order_Id).StoreOrders.Add(storeOrder);
                        storeOrder.DeliveryFee = storeOrder.Total - storeOrder.Subtotal;
                    }
                    return Ok(new CustomResponse<OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = orderHistory });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("RepeatOrder")]
        public async Task<IHttpActionResult> RepeatOrder(string orderId)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var Order = ctx.Orders.AsNoTracking().Include(x => x.StoreOrders.Select(x1 => x1.Order_Items)).Where(x => x.OrderNo == orderId).FirstOrDefault();



                    ctx.Orders.Add(Order);
                    ctx.SaveChanges();
                    return Ok(new CustomResponse<Order> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = Order });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }



        }

        [HttpPost]
        [Route("RequestGetCloth")]
        public async Task<IHttpActionResult> RequestGetCloth(RequestClothBindingModel model)
        {
            try
            {
                var Response = new LaundryRequest();
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var TodayDate = DateTime.Today.Day;
                    var checkRequest = ctx.LaundryRequest.FirstOrDefault(x => x.User_Id == model.User_Id && x.Store_Id == model.Store_Id && x.RequestDate.Day == TodayDate);
                    if (checkRequest == null)
                    {
                        LaundryRequest Request = new LaundryRequest
                        {
                            User_Id = model.User_Id,
                            Store_Id = model.Store_Id,
                            Name = "Get Cloth Request",
                            RequestDate = DateTime.Now,
                            isDeleted = false,
                            Status = Convert.ToInt16(Global.ClothRequestTypes.Pending)

                        };
                        ctx.LaundryRequest.Add(Request);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        return Ok(new CustomResponse<Error> { Message = "Conflict", StatusCode = (int)HttpStatusCode.Conflict, Result = new Error { ErrorMessage = "You already requested to Get Cloths for today." } });
                    }
                }
                return Ok(new CustomResponse<LaundryRequest> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = Response });

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpPost]
        [Route("GetCartMobile")]
        public async Task<IHttpActionResult> GetCartMobile(MobileOrderViewModel model)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (System.Web.HttpContext.Current.Request.Params["Store"] != null)
                        model.Store = JsonConvert.DeserializeObject<List<MobileCart>>(System.Web.HttpContext.Current.Request.Params["Store"]);
                    var UserAddress = ctx.UserAddresses.FirstOrDefault(x => x.User_ID == model.User_Id && x.IsPrimary == true && x.IsDeleted == false);
                    if (UserAddress == null)
                    {
                        UserAddress = ctx.UserAddresses.FirstOrDefault(x => x.User_ID == model.User_Id && x.IsDeleted==false );
                        if(UserAddress == null)
                        {
                            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "User addresses not found." } });
                        }
                    }
                    model.Address = UserAddress;

                    var UserCreditCard = ctx.CreditCards.FirstOrDefault(x => x.User_ID == model.User_Id && x.Is_Primary == 1 && x.is_delete == false);
                    if (UserCreditCard == null)
                    {
                        UserCreditCard = ctx.CreditCards.FirstOrDefault(x => x.User_ID == model.User_Id && x.is_delete== false);
                        if (UserCreditCard == null)
                        {
                            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "User credit card not found." } });
                        }
                    }
                    model.CreditCard = UserCreditCard;


                    var storeIds = model.Store.Select(x => x.storeId).Distinct();

                    var storeBusinessTypes = ctx.Stores.Where(x => storeIds.Contains(x.Id));

                    var Types=storeBusinessTypes.GroupBy(x => x.BusinessType).Select(x=>x.Key);

                    foreach (var item in Types)
                    {
                        model.OrderSummary.Tax = model.OrderSummary.Tax + ctx.BusinessTypeTax.FirstOrDefault(x => item.Contains(x.BusinessType)).Tax;


                    }
             
                    foreach (var store in model.Store)
                    {
                        var Store = ctx.Stores.FirstOrDefault(x => x.Id == store.storeId);

                        store.minDeliveryCharges = Store.MinDeliveryCharges;
                        store.minDeliveryTime = Store.MinDeliveryTime;
                        store.minOrderPrice = Store.MinOrderPrice;

                        foreach (var product in store.products)
                        {
                            store.StoreSubTotal = store.StoreSubTotal + (ctx.Products.FirstOrDefault(x => x.Id == product.Id).Price * product.quantity);
                        }
                       
                    }

                    foreach (var SingleStore in model.Store)
                    {
                        SingleStore.StoreTotal = Convert.ToDouble(SingleStore.StoreSubTotal.Value) + Convert.ToDouble(SingleStore.minDeliveryCharges.Value);
                    }

                    foreach (var SingleStore in model.Store)
                    {
                        model.OrderSummary.SubTotal = model.OrderSummary.SubTotal + SingleStore.StoreTotal.Value;
                        model.OrderSummary.DeliveryFee = model.OrderSummary.DeliveryFee + Convert.ToDouble(SingleStore.minDeliveryCharges);
                        model.OrderSummary.SubTotalWDF = model.OrderSummary.SubTotal - model.OrderSummary.DeliveryFee;
                    }
              
                    if(DunkeySettings.Tip == 0)
                    {
                        DunkeySettings.LoadSettings();
                    }
                    model.OrderSummary.Tip = Math.Round((model.OrderSummary.SubTotal / 100) * DunkeySettings.Tip, 4);
                    model.OrderSummary.Total = model.OrderSummary.SubTotal + model.OrderSummary.Tip + model.OrderSummary.Tax;
                    foreach (var store in model.Store)
                    {
                        if (store.minDeliveryCharges != null)
                        {
                            store.products.Add(new productslist { Name = "Delivery Fee", Price = Convert.ToDouble(store.minDeliveryCharges.Value), Store_id = store.storeId });
                        }
                    }

                }
                return Ok(new CustomResponse<MobileOrderViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("GetCartMobileDemo")]
        public async Task<IHttpActionResult> GetCartMobileDemo(MobileOrderViewModel model)
        {
            try
            {
                if (System.Web.HttpContext.Current.Request.Params["Store"] != null)
                    model.Store = JsonConvert.DeserializeObject<List<MobileCart>>(System.Web.HttpContext.Current.Request.Params["Store"]);

                return Ok(new CustomResponse<MobileOrderViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("testdatetime")]
        public async Task<IHttpActionResult> testdatetime()
        {
            try
            {
                using (DunkeyContext ctx=new DunkeyContext())
                {

                    var UTCnow = DateTime.UtcNow;
                    var time = ctx.Stores.FirstOrDefault(x => x.Id == 1).MinDeliveryTime;
                    var test=Convert.ToDateTime(time);
                    var tim = TimeSpan.FromMilliseconds(Convert.ToDouble(time));


                    //var deliveryTime=tim.TimeOfDay;

                }
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "" });

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

    }
}
