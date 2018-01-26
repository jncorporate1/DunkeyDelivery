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

                    returnModel.Categories= ctx.Categories.Where(x => x.Store.BusinessType == Utility.Global.Constants.Laundry && x.ParentCategoryId==0).ToList();
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
                    return Ok(new CustomResponse<LaundryProductsViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

    }
}
