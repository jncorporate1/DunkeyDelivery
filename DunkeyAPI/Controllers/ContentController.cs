using DAL;
using DunkeyAPI.ViewModels;
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
    [RoutePrefix("api/Content")]
    public class ContentController : ApiController
    {
        [DunkeyDelivery.Authorize("SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("SearchContent")]
        public async Task<IHttpActionResult> SearchContent(string Name,short? is_partner=0,short? is_investor=0,short? is_press=0)
        {
            try
            {
                var Contributors = new List<Contributors>();
          
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    string conditions = string.Empty;

                    if (string.IsNullOrEmpty(Name))
                    {
                        
                        if (is_investor == 1)
                            Contributors = ctx.Contributors.Where(x => x.is_invester == 1 && x.is_deleted==0).ToList();
                        else if (is_partner == 1)
                            Contributors = ctx.Contributors.Where(x => x.is_partner == 1 && x.is_deleted == 0).ToList();
                        else if (is_press == 1)
                            Contributors = ctx.Contributors.Where(x => x.is_press == 1 && x.is_deleted == 0).ToList();
                        else
                            Contributors = ctx.Contributors.Where(x=>x.is_deleted==0).ToList();
                    }
                    else
                    {
                        if (is_investor == 1)
                            Contributors = ctx.Contributors.Where(x => x.is_invester == 1 && x.Name.Contains(Name) && x.is_deleted == 0).ToList();
                        else if (is_partner == 1)
                            Contributors = ctx.Contributors.Where(x => x.is_partner == 1 && x.Name.Contains(Name) && x.is_deleted == 0).ToList();
                        else if (is_press == 1)
                            Contributors = ctx.Contributors.Where(x => x.is_press == 1 && x.Name.Contains(Name) && x.is_deleted == 0).ToList();
                        else
                            Contributors = ctx.Contributors.Where(x => x.Name.Contains(Name) && x.is_deleted == 0).ToList();
                    }

                    foreach (var item in Contributors)
                    {
                        if (item.is_invester == 1)
                            item.Type = "Investor";
                        else if (item.is_partner == 1)
                            item.Type = "Partner";
                        else if (item.is_press == 1)
                            item.Type = "Press";
                    }

                    if(Contributors == null)
                    {
                        return Ok(new CustomResponse<Error> { Message = ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Record not found" } });

                    }
                    else
                    {
                        return Ok(new CustomResponse<ContentListViewModel> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new ContentListViewModel { Contributors = Contributors } });

                    }


                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("DeleteEntity")]
        [DunkeyDelivery.Authorize("SuperAdmin", "ApplicationAdmin")]
        public async Task<IHttpActionResult> DeleteEntity(int Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var Contributor = ctx.Contributors.FirstOrDefault(x => x.Id == Id);
                    if (Contributor != null)
                    {
                        Contributor.is_deleted = 1;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });

                    }
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Not Found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Result = new Error { ErrorMessage = "Record not found." }
                    });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

    }
}
