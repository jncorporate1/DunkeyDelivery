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
        public async Task<IHttpActionResult> SearchContent(string Name, short? is_partner = 0, short? is_investor = 0, short? is_press = 0)
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
                            Contributors = ctx.Contributors.Where(x => x.is_invester == 1 && x.is_deleted == 0).ToList();
                        else if (is_partner == 1)
                            Contributors = ctx.Contributors.Where(x => x.is_partner == 1 && x.is_deleted == 0).ToList();
                        else if (is_press == 1)
                            Contributors = ctx.Contributors.Where(x => x.is_press == 1 && x.is_deleted == 0).ToList();
                        else
                            Contributors = ctx.Contributors.Where(x => x.is_deleted == 0).ToList();
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

                    if (Contributors == null)
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



        [HttpGet]
        [Route("GetContentByType")]
        public async Task<IHttpActionResult> GetContentByType(int Type)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {

                    var content = ctx.Content.FirstOrDefault(x => x.Type == Type);
                    if (content != null)
                    {
                        return Ok(new CustomResponse<Content> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = content });

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

        [HttpPost]
        [Route("GetContentEntities")]
        [DunkeyDelivery.Authorize("SuperAdmin", "ApplicationAdmin")]
        public async Task<IHttpActionResult> GetContentEntities(Content BindingModel)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {

                    var content = ctx.Content.FirstOrDefault(x => x.Type == BindingModel.Type);
                    if (BindingModel.Id == 0)
                    {
                        return Ok(new CustomResponse<Content> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = content });
                    }
                    if (content != null)
                    {
                        content.VideoUrl = BindingModel.VideoUrl;
                        content.Description = BindingModel.Description;
                        content.Heading = BindingModel.Heading;
                        content.ImageUrl = BindingModel.ImageUrl;
                        content.Title = BindingModel.Title;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<Content> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = content });

                    }
                    else
                    {
                        Content NewContent = new Content
                        {
                            Description = BindingModel.Description,
                            VideoUrl = BindingModel.VideoUrl,
                            Type = (int)DunkeyDelivery.Content.Types.AboutUs,
                            Heading = BindingModel.Heading,
                            Title = BindingModel.Title,
                            ImageUrl = BindingModel.ImageUrl

                        };

                        ctx.Content.Add(NewContent);
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<Content> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = NewContent });

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


        [HttpPost]
        [Route("FAQ")]
        public async Task<IHttpActionResult> FAQ(FAQViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var content = new FAQ();
                    if (model.Id != 0)
                    {
                        content = ctx.FAQ.FirstOrDefault(x => x.Id == model.Id);
                    }

                    if (content.Question != null)
                    {

                        content.Question = model.Question;
                        content.Answer = model.Answer;
                        //content.Type= model.Type;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<FAQ> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = content });

                    }
                    else
                    {
                        FAQ NewFAQ = new FAQ
                        {
                            Question = model.Question,
                            Answer = model.Answer,
                            Type = model.Type,
                            isDeleted = false,
                        };

                        ctx.FAQ.Add(NewFAQ);
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<FAQ> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = NewFAQ });

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

        [HttpGet]
        [Route("DeleteFAQ")]

        public async Task<IHttpActionResult> DeleteFAQ(int Id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var FAQ = ctx.FAQ.FirstOrDefault(x => x.Id == Id);
                    FAQ.isDeleted = true;
                    ctx.SaveChanges();
                    return Ok(new CustomResponse<FAQ> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = FAQ });

                }
                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                {
                    Message = "Not Found",
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Result = new Error { ErrorMessage = "Record not found." }
                });

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }
    }
}
