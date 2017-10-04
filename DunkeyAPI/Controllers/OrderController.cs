using DAL;
using DunkeyAPI.ExtensionMethods;
using DunkeyAPI.Utility;
using DunkeyAPI.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

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




    }
}
