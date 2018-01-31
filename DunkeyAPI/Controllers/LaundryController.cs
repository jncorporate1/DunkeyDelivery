using DAL;
using DunkeyAPI.Utility;
using DunkeyAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Laundry")]
    public class LaundryController : ApiController
    {
        [HttpGet]
        [Route("GetParentCategories")]
        public async Task<IHttpActionResult> GetParentCategories(int Store_id,int? Page = 0, int? Items = 10)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    LaundryCategoriesViewModel returnModel = new LaundryCategoriesViewModel();

                    returnModel.Categories= ctx.Categories.Where(x => x.Store.BusinessType == Utility.Global.Constants.Laundry && x.ParentCategoryId==0 && x.Store_Id==Store_id).ToList();
                    return Ok(new CustomResponse<LaundryCategoriesViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });
                    
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetCategoryItems")]
        public async Task<IHttpActionResult> GetCategoryItems(int Store_id, int ParentCategory_id, int? Page = 0, int? Items = 10)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    LaundryProductsViewModel returnModel = new LaundryProductsViewModel();

                    returnModel.Products = ctx.Products.Where(x => x.Store.BusinessType == Utility.Global.Constants.Laundry && x.Store.Id==Store_id && x.Category_Id == ParentCategory_id).ToList();

                    foreach (var product in returnModel.Products)
                    {
                        var prod= ctx.Stores.FirstOrDefault(x => x.Id == product.Store_Id);
                        product.MinDeliveryTime = prod.MinDeliveryTime;
                        product.BusinessName = prod.BusinessName;
                    }

                    return Ok(new CustomResponse<LaundryProductsViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }



        [HttpPost]
        [Route("RequestGetClothMobile")]
        public async Task<IHttpActionResult> RequestGetClothMobile(RequestClothMobileBindingModel model)
        {
            try
            {
                var Response = new LaundryRequest();
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var pickupDateTime = DateTime.Now;
                    var TodayDate = DateTime.Today.Day;
                    var checkRequest = ctx.LaundryRequest.FirstOrDefault(x => x.User_Id == model.User_Id && x.Store_Id == model.Store_Id && x.RequestDate.Day == TodayDate);
                    if (checkRequest == null)
                    {
                        if (model.PickUpTime.HasValue)
                        {
                            pickupDateTime = model.PickUpTime.Value;
                        }
                        LaundryRequest Request = new LaundryRequest
                        {
                            User_Id = model.User_Id,
                            Store_Id = model.Store_Id,
                            Note = model.AdditionalNote,
                            Weight=model.Weight,                            
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

    }
}
