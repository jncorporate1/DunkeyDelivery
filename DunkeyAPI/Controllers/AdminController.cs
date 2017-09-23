using DAL;
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
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        [HttpGet]
        [Route("GetDashboardStats")]
        public IHttpActionResult GetDashboardStats()
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                WebDashboardStatsViewModel model = new WebDashboardStatsViewModel
                {
                    TotalUsers=ctx.Users.Count(),
                    TotalStores=ctx.Stores.Count(),
                    TotalProducts = ctx.Stores.Count()
                    

                };

                CustomResponse<WebDashboardStatsViewModel> response = new CustomResponse<WebDashboardStatsViewModel>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = model
                };
                return Ok(response);



            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }



    }
}
