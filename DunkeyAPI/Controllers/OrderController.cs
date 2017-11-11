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

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (model.Cart.CartItems.Count() > 0)
                {
                    order = new Order();
                    using (DunkeyContext ctx = new DunkeyContext())
                    {
                        order.MakeOrder(model, ctx);
                        
                        order.DeliveryTime_From = DateTime.Now;
                        order.DeliveryTime_To = DateTime.Now;

                        //Charge User
                        StripeCharge stripeCharge = DunkeyDelivery.Utility.GetStripeChargeInfo(model.StripeEmail, model.StripeAccessToken, Convert.ToInt32(order.Total));

                        if (stripeCharge.Status != "succeeded")
                        {
                            return Ok(new CustomResponse<Error> { Message = "Payment Failed", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "We are unable to process your payments. Please try sometime later" } });
                        }

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
                        orderQuery = @"SELECT *, Users.FullName as UserFullName FROM Orders join Users on Users.ID = Orders.User_ID where Orders.User_Id = " + UserId + @" and Orders.IsDeleted = 0  ORDER BY Orders.id OFFSET " + PageNo * PageSize + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY;";
                    }
                    else
                    {
                        orderQuery = @"SELECT *, Users.FullName as UserFullName FROM Orders 
						join Users on Users.ID = Orders.User_ID
						where Orders.DeliveryMan_Id = " + UserId + @" and Orders.IsDeleted = 0  
						ORDER BY Orders.id OFFSET " + PageNo * PageSize + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY;";
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
    }
}
