using AutoMapper;
using DAL;
using DunkeyAPI.Models;
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
using System.Data.Entity;


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
                model.User_ID = Convert.ToInt32(httpRequest.Params["User_Id"]);
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
                            Title = model.Title,
                            CategoryType = model.CategoryType,
                            Description = model.Description,
                            ImageUrl = DunkeyDelivery.Utility.BaseUrl + ConfigurationManager.AppSettings["UserImageFolderPath"] + Path.GetFileName(newFullPath),
                            DateOfPosting = model.DateOfPosting,
                            User_ID = model.User_ID

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

        [HttpGet]
        [Route("InsertComments")]
        public IHttpActionResult InsertComments(string Email, string Message, int Post_id, int UserId)
        {

            try
            {

                if (string.IsNullOrEmpty(Email))
                {

                }
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    BlogComments Comment = new BlogComments
                    {
                        Message = Message,
                        PostedDate = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        User_Id = UserId
                    };
                    var BlogData = ctx.BlogPosts.Where(x => x.Id == Post_id).FirstOrDefault();
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
        public IHttpActionResult GetBlogPosts(int Page=0,int Items=6)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {

                    BlogPostListViewModel model = new BlogPostListViewModel();
                    model.BlogPosts = ctx.BlogPosts.Include(c => c.User).Skip(Page*Items).Take(Items).ToList();

                    foreach (var post in model.BlogPosts)
                    {
                        post.TotalComments = ctx.BlogComments.Count(x=>x.Post_Id==post.Id);
                    }


                    // popular categories
                    var query = "select CategoryType, COUNT(CategoryType)AS TotalCount from BlogPosts GROUP BY BlogPosts.CategoryType  ORDER BY TotalCount DESC OFFSET 0 ROWS  FETCH NEXT 3 ROWS ONLY";
                    var popularCategories = ctx.Database.SqlQuery<popularCategories>(query).ToList();
                    model.popularCategories = popularCategories;

                    CustomResponse<BlogPostListViewModel> response = new CustomResponse<BlogPostListViewModel>
                    {
                        Message = DunkeyDelivery.Global.SuccessMessage,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };

                    return Ok(response);

                    //return Ok(new CustomResponse<BlogPostListViewModel>
                    //{
                    //    Message = "Success",
                    //    StatusCode = (int)HttpStatusCode.OK,
                    //    Result = new BlogPostListViewModel
                    //    {
                    //        BlogPosts = ctx.BlogPosts.Include(c => c.User).Include(c => c.BlogComments).Select(x => new BlogPosts
                    //        {
                    //            TotalComments = x.BlogComments.Count,
                    //            Id = x.Id,
                    //            Title= x.Title,
                    //            is_popular=x.is_popular,
                    //            ImageUrl= x.ImageUrl,
                    //            User= x.User,
                    //            Description=x.Description,
                    //            DateOfPosting= x.DateOfPosting,
                    //            CategoryType= x.CategoryType
                    //        }).ToList()
                    //    }
                    //});
                }

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
