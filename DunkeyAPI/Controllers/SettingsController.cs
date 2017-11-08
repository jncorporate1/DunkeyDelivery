using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static DunkeyAPI.Utility.Global;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Admin")]
    public class SettingsController : ApiController
    {
    //    [DunkeyDelivery.Authorize("SuperAdmin", "ApplicationAdmin")]
    //    [HttpGet]
    //    [Route("GetContentData")]
    //    public async Task<IHttpActionResult> GetContentData()
    //    {
    //        try
    //        {
    //            using (DunkeyContext ctx = new DunkeyContext())
    //            {
                   
    //                CustomResponse<> response = new CustomResponse<>
    //                {
    //                    Message = ResponseMessages.Success,
    //                    StatusCode = (int)HttpStatusCode.OK,
    //                    Result = 
    //                };
    //                return Ok(response);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(DunkeyDelivery.Utility.LogError(ex));
    //        }
    //    }
    }
}
