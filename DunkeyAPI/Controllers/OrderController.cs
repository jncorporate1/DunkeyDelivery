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
					order.MakeOrder(model);

					using (DunkeyContext ctx = new DunkeyContext())
					{
						order.DeliveryTime_From = DateTime.Now;
						order.DeliveryTime_To = DateTime.Now;

						ctx.Orders.Add(order);
						await ctx.SaveChangesAsync();
					}

					return Ok(new CustomResponse<OrderSummaryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new OrderSummaryViewModel(order) });
				}
				else if (System.Web.HttpContext.Current.Request.Params["Cart"] != null)
				{
					model.Cart = JsonConvert.DeserializeObject<CartViewModel>(System.Web.HttpContext.Current.Request.Params["Cart"]);
					if (model.Cart.CartItems.Count() > 0)
					{
						order = new Order();
						order.MakeOrder(model);

						using (DunkeyContext ctx = new DunkeyContext())
						{
							ctx.Orders.Add(order);
							await ctx.SaveChangesAsync();
						}

						return Ok(new CustomResponse<OrderSummaryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new OrderSummaryViewModel(order) });
					}
					else
						return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "No items in the cart." } });
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
					//var orderItemsQuery = @"
					//                    SELECT
					//                      CASE
					//                        WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Id
					//                        WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Id
					//                        WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.Id
					//                        WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.Id
					//                      END AS ItemId,
					//                      CASE
					//                        WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Name
					//                        WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Name
					//                        WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.Name
					//                        WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.Name
					//                      END AS Name,
					//                      CASE
					//                        WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Price
					//                        WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Price
					//                        WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.DiscountedPrice
					//                        WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.DiscountedPrice
					//                      END AS Price,
					//                      CASE
					//                        WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Image

					//                        WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.ImageUrl
					//                        WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.ImageUrl
					//                      END AS ImageUrl,
					//                    CASE 
					//                    When ISNULL(Order_Items.Product_Id, 0) <> 0 Then Products.Id
					//                    When ISNULL(Order_Items.Package_Id, 0) <> 0 Then Packages.Id
					//                    When ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 Then Offer_Products.Id
					//                    When ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 Then Offer_Packages.Id
					//                    END as ItemId, 
					//                      Order_Items.Id,
					//                      Order_Items.Qty,

					//                      Order_Items.StoreOrder_Id
					//                    FROM Order_Items
					//                    LEFT JOIN products
					//                      ON products.Id = Order_Items.Product_Id
					//                    LEFT JOIN Packages
					//                      ON Packages.Id = Order_Items.Package_Id
					//                    LEFT JOIN Offer_Products
					//                      ON Offer_Products.Id = Order_Items.Offer_Product_Id
					//                    LEFT JOIN Offer_Packages
					//                      ON Offer_Packages.Id = Order_Items.Offer_Package_Id
					//                    WHERE StoreOrder_Id IN (" + storeOrderIds + ")";
					var orderItemsQuery = @"SELECT   

CASE   WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Id 
 WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Id 
  WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.Id  
   WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.Id   END AS ItemId,  
   CASE
   WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Name
   WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Name
   WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.Name
   WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.Name
   END AS Name,
	CASE
	WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Price
	WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Price
	WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.DiscountedPrice
	WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.DiscountedPrice
	END AS Price,
	CASE
	WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Image
	WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.ImageUrl
	WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.ImageUrl
	END AS ImageUrl,
	 CASE 
	 When ISNULL(Order_Items.Product_Id, 0) <> 0 Then Products.Id
		 When ISNULL(Order_Items.Package_Id, 0) <> 0 Then Packages.Id
		  When ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 Then Offer_Products.Id
		  When ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 Then Offer_Packages.Id
		   END as ItemId, Order_Items.Id,   
		  Order_Items.Qty, Order_Items.StoreOrder_Id FROM Order_Items
		  LEFT JOIN products
		  ON products.Id = Order_Items.Product_Id
			 LEFT JOIN Packages
			   ON Packages.Id = Order_Items.Package_Id
				LEFT JOIN Offer_Products
				  ON Offer_Products.Id = Order_Items.Offer_Product_Id
					  LEFT JOIN Offer_Packages
						 ON Offer_Packages.Id = Order_Items.Offer_Package_Id WHERE StoreOrder_Id IN  (" + storeOrderIds + ")";
					#endregion

					var orderItems = ctx.Database.SqlQuery<OrderItemViewModel>(orderItemsQuery).ToList();

					//var userFavourites = ctx.Favourites.Where(x => x.User_ID == UserId && x.IsDeleted == false).ToList();

					//foreach (var orderItem in orderItems)
					//{
					//	orderItem.Weight = Convert.ToString(orderItem.WeightInGrams) + " gm";

					//                   if (userFavourites.Any(x => x.Product_Id == orderItem.Id))
					//                       orderItem.IsFavourite = true;
					//                   else
					//                       orderItem.IsFavourite = false;
					//               }

					foreach (var orderItem in orderItems)
					{
						storeOrders.FirstOrDefault(x => x.Id == orderItem.StoreOrder_Id).OrderItems.Add(orderItem);
					}

					foreach (var storeOrder in storeOrders)
					{
						orderHistory.orders.FirstOrDefault(x => x.Id == storeOrder.Order_Id).StoreOrders.Add(storeOrder);
					}

					return Ok(new CustomResponse<OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = orderHistory });

					#region CommentedOldLogic
					//OrdersHistoryViewModel ordersHistoryViewModel = new OrdersHistoryViewModel();

					//if (IsCurrentOrder)
					//{
					//    if (SignInType == (int)RoleTypes.User)
					//    {
					//        ordersHistoryViewModel.Count = ctx.Orders.Count(x => x.User_ID == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed);

					//        ordersHistoryViewModel.orders = ctx.Orders.Include(x => x.StoreOrders.Select(x1 => x1.Order_Items.Select(x2 => x2.Product.Store)))
					//            .Where(x => x.User_ID == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed).OrderBy(x => x.Id).Page(PageSize, PageNo).ToList();
					//    }
					//    else
					//    {
					//        ordersHistoryViewModel.Count = ctx.Orders.Count(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed);

					//        ordersHistoryViewModel.orders = ctx.Orders.Include(x => x.StoreOrders.Select(x1 => x1.Order_Items.Select(x2 => x2.Product.Store)))
					//         .Where(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed).OrderBy(x => x.Id).Page(PageSize, PageNo).ToList();
					//    }
					//}
					//else
					//{
					//    if (SignInType == (int)RoleTypes.User)
					//    {
					//        ordersHistoryViewModel.Count = ctx.Orders.Count(x => x.User_ID == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed);

					//        ordersHistoryViewModel.orders = ctx.Orders.Include(x => x.StoreOrders.Select(x1 => x1.Order_Items.Select(x2 => x2.Product.Store)))
					//            .Where(x => x.User_ID == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed).OrderBy(x => x.Id).Page(PageSize, PageNo).ToList();
					//    }
					//    else
					//    {
					//        ordersHistoryViewModel.Count = ctx.Orders.Count(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed);

					//        ordersHistoryViewModel.orders = ctx.Orders.Include(x => x.StoreOrders.Select(x1 => x1.Order_Items.Select(x2 => x2.Product.Store)))
					//            .Where(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed).OrderBy(x => x.Id).Page(PageSize, PageNo).ToList();
					//    }
					//}
					//return Ok(new CustomResponse<OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ordersHistoryViewModel }); 
					#endregion
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
