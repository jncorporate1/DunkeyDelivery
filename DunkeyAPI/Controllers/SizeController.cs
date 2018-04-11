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
        public async Task<IHttpActionResult> GetAllUnits(int? Type=0)
        {
            // Type will be 0 in case of wine and liquor and 1 for beer
            // for beer i will return static units where for wine and liquor i will return result from data base
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                List<SizesUnits> model = new List<SizesUnits>();
                if (Type.Value == 1)
                {
                    model.Add(new SizesUnits { Unit = "6 Pack, 12oZ Bottle" });
                    model.Add(new SizesUnits { Unit = "12 Pack, 12oZ Bottle" });
                    model.Add(new SizesUnits { Unit = "24 Pack, 12oZ Bottle" });
                }
                else
                {
                    model = ctx.SizesUnits.OrderBy(x => x.Id).ToList();
                }

                CustomResponse<IEnumerable<SizesUnits>> response = new CustomResponse<IEnumerable<SizesUnits>>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result =model
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
