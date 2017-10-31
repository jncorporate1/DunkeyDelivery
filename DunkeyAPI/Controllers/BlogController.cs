using AutoMapper;
using DAL;
using DunkeyAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Blog")]
    public class BlogController : ApiController
    {
        [HttpPost]
        [Route("InsertBlog")]
        public IHttpActionResult InsertBlog()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;
                BlogViewModel model = new BlogViewModel();

                model.Title = httpRequest.Params["Title"];
                model.CategoryType = httpRequest.Params["CategoryType"];
                model.DateOfPosting = Convert.ToDateTime(httpRequest.Params["DateOfPosting"]);
                model.Description = httpRequest.Params["Description"];
                model.User_ID=Convert.ToInt32(httpRequest.Params["User_Id"]);
                Validate(model);

                #region Validations
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multipart data is not included in request." }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image." }
                    });
                }
                #endregion

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (ctx.BlogPosts.Any(x => x.Title == model.Title && x.CategoryType == model.CategoryType))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "Post with under this category already exists" }
                        });

                    }
                    else
                    {
                        #region ImageSaving
                        var postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            //int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            //var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                            var ext = Path.GetExtension(postedFile.FileName);
                            var extension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(extension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png." }
                                });
                            }
                            else if (postedFile.ContentLength > DunkeyDelivery.Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + DunkeyDelivery.Global.ImageSize + "." }
                                });
                            }
                            else
                            {
                                int count = 1;
                                fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["BlogImageFolderPath"] + postedFile.FileName);

                                while (File.Exists(newFullPath))
                                {
                                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["BlogImageFolderPath"] + tempFileName + extension);
                                }

                                postedFile.SaveAs(newFullPath);
                            }
                        }
                        #endregion


                        BlogPosts post = new BlogPosts
                        {
                            Title=model.Title,
                            CategoryType=model.CategoryType,
                            Description=model.Description,
                            ImageUrl = DunkeyDelivery.Utility.BaseUrl + ConfigurationManager.AppSettings["UserImageFolderPath"] + Path.GetFileName(newFullPath),
                            DateOfPosting=model.DateOfPosting,
                            User_ID=model.User_ID
                        
                        };
                        ctx.BlogPosts.Add(post);
                        ctx.SaveChanges();

                        CustomResponse<BlogPosts> response = new CustomResponse<BlogPosts>
                        {
                            Message = DunkeyDelivery.Global.SuccessMessage,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = post
                        };

                        return Ok(response);

                    }




                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpPost]
        [Route("InsertComments")]
        public IHttpActionResult InsertComments(string Email,string Message,int Post_id)
        {

            try
            {

                if (string.IsNullOrEmpty(Email))
                {

                }
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    BlogComments Comment = new BlogComments {
                        Message=Message,
                        PostedDate=DateTime.Now,
                        CreatedDate=DateTime.Now
                    };
                    var BlogData=ctx.BlogPosts.Include().Where(x => x.Id==Post_id && x.User.Email==Email).FirstOrDefault();
                    BlogData.BlogComments.Add(Comment);
                    ctx.SaveChanges();

                    CustomResponse<BlogComments> response = new CustomResponse<BlogComments>
                    {
                        Message = DunkeyDelivery.Global.SuccessMessage,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = Comment
                    };

                    return Ok(response);


                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetBlogPosts")]
        public IHttpActionResult GetBlogPosts()
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                List<BlogViewModel> postsModel = new List<BlogViewModel>();
                var res = ctx.BlogPosts.ToList();

                var f = Mapper.Map<List<BlogViewModel>>(res);
                postsModel = f;
                CustomResponse<List<BlogViewModel>> response = new CustomResponse<List<BlogViewModel>>
                { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = postsModel };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("GetBlogPostsById")]
        public IHttpActionResult GetBlogPostsById(int id)
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
              
                var res = ctx.BlogPosts.FirstOrDefault();

                CustomResponse<BlogPosts> response = new CustomResponse<BlogPosts>
                { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = res };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

    }
}
