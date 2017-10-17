using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using System.Data.Entity;
using static DunkeyAPI.Utility.Global;
using static DunkeyDelivery.Utility;

namespace DunkeyAPI.Controllers
{
   // [Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
    [RoutePrefix("api")]
    public class CommonController : ApiController
    {
        [HttpGet]
        [Route("GetEntityById")]
        public async Task<IHttpActionResult> GetEntityById(int EntityType, int Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    switch (EntityType)
                    {
                        case (int)DunkeyEntityTypes.Product:
                            return Ok(new CustomResponse<Product> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Products.FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });

                        case (int)DunkeyEntityTypes.Category:
                            return Ok(new CustomResponse<Category> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Categories.FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });

                        case (int)DunkeyEntityTypes.Store:
                            return Ok(new CustomResponse<Store> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Stores.Include(x => x.StoreDeliveryHours).FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });

                        case (int)DunkeyEntityTypes.Package:
                            return Ok(new CustomResponse<Package> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Packages.FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });

                        case (int)DunkeyEntityTypes.Admin:
                            return Ok(new CustomResponse<DAL.Admin> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Admins.FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });

                        case (int)DunkeyEntityTypes.Offer:
                            return Ok(new CustomResponse<DAL.Offer> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Offers.FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });

                        default:
                            return Ok(new CustomResponse<Error> { Message = ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid entity type" } });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }
    }
}
