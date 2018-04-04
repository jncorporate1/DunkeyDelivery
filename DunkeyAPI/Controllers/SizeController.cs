using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Size")]
    public class SizeController : ApiController
    {
        [HttpGet]
        [Route("GetAllUnits")]
        public async Task<IHttpActionResult> GetAllUnits()
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();

                CustomResponse<IEnumerable<SizesUnits>> response = new CustomResponse<IEnumerable<SizesUnits>>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = ctx.SizesUnits.OrderBy(x => x.Id).ToList()
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
