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
using System.Threading.Tasks;
using static DunkeyAPI.Utility.Global;
using System.Web.Http.Results;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Blog")]
    public class BlogController : ApiController
    {
        [HttpPost]
        [Route("InsertBlog")]
        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        public IHttpActionResult InsertBlog()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;
                var extension = string.Empty;
                BlogPosts model = new BlogPosts();
                BlogPosts existingModel = new BlogPosts();
                model.Title = httpRequest.Params["Title"];
                model.CategoryType = httpRequest.Params["CategoryType"];
                model.DateOfPosting = Convert.ToDateTime(httpRequest.Params["DateOfPosting"]);
                model.Description = httpRequest.Params["Description"];
                model.Admin_ID = Convert.ToInt32(httpRequest.Params["User_Id"]);
                if (httpRequest.Params["Id"] != null)
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);

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

                    if (model.Id == 0)
                    {
                        if (ctx.BlogPosts.Any(x => x.Title == model.Title && x.CategoryType == model.CategoryType && x.is_deleted == 0))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Post with under this category already exists" }
                            });

                        }
                        //else
                        //{

                        //    BlogPosts post = new BlogPosts
                        //    {
                        //        Title = model.Title,
                        //        CategoryType = model.CategoryType,
                        //        Description = model.Description,
                        //        ImageUrl = DunkeyDelivery.Utility.BaseUrl + ConfigurationManager.AppSettings["BlogImageFolderPath"] + Path.GetFileName(newFullPath),
                        //        DateOfPosting = model.DateOfPosting,
                        //        Admin_ID = model.Admin_ID,
                        //        is_deleted=0

                        //    };
                        //    ctx.BlogPosts.Add(model);
                        //    ctx.SaveChanges();

                        //    CustomResponse<BlogPosts> response = new CustomResponse<BlogPosts>
                        //    {
                        //        Message = DunkeyDelivery.Global.SuccessMessage,
                        //        StatusCode = (int)HttpStatusCode.OK,
                        //        Result = post
                        //    };

                        //    return Ok(response);
                        //    }
                    }
                    else
                    {
                        existingModel = ctx.BlogPosts.FirstOrDefault(x => x.Id == model.Id);


                    }

                    #region ImageSaving
                    string fileExtension = string.Empty;
                    HttpPostedFile postedFile = null;

                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = Path.GetExtension(postedFile.FileName);
                            fileExtension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(fileExtension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > DunkeyDelivery.Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + DunkeyDelivery.Global.ImageSize }
                                });
                            }
                            else
                            {
                                //int count = 1;
                                //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + postedFile.FileName);

                                //while (File.Exists(newFullPath))
                                //{
                                //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + tempFileName + extension);
                                //}
                                //postedFile.SaveAs(newFullPath);
                            }
                        }
                        //model.ImageUrl = ConfigurationManager.AppSettings["AdminImageFolderPath"] + Path.GetFileName(newFullPath);
                    }

                    #endregion

                    if (model.Id == 0)
                    {
                        //BlogPosts post = new BlogPosts
                        //{
                        //    Title = model.Title,
                        //    CategoryType = model.CategoryType,
                        //    Description = model.Description,
                        //    DateOfPosting = model.DateOfPosting,
                        //    Admin_ID = model.Admin_ID,
                        //    is_deleted = 0

                        //};
                        ctx.BlogPosts.Add(model);
                        ctx.SaveChanges();
                        model.ImageUrl = ConfigurationManager.AppSettings["BlogImageFolderPath"] + model.Id + fileExtension;
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["BlogImageFolderPath"] + model.Id + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (model.ImageDeletedOnEdit == false)
                            {
                                model.ImageUrl = existingModel.ImageUrl;
                            }
                        }
                        else
                        {
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["BlogImageFolderPath"] + model.Id + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            model.ImageUrl = ConfigurationManager.AppSettings["BlogImageFolderPath"] + model.Id + fileExtension;
                        }

                        ctx.Entry(existingModel).CurrentValues.SetValues(model);
                        ctx.SaveChanges();
                    }

                    CustomResponse<BlogPosts> response = new CustomResponse<BlogPosts>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
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
        [Route("InsertComments")]
        public IHttpActionResult InsertComments(string Message, int Post_id, int UserId)
        {

            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (UserId==0)
                    {
                       UserId= ctx.Users.FirstOrDefault(x => x.Email == DunkeyDelivery.Utility.GuestEmail).Id;
                    }

                    BlogComments Comment = new BlogComments
                    {
                        Message = Message,
                        PostedDate = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        User_Id = UserId,
                        Post_Id=Post_id
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
        public IHttpActionResult GetBlogPosts(int Page = 0, int Items = 6)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {

                    BlogPostListViewModel model = new BlogPostListViewModel();
                    model.BlogPosts = ctx.BlogPosts.Include(c => c.Admin).OrderBy(x => x.Title).Skip(Page * Items).Take(Items).ToList();

                    foreach (var post in model.BlogPosts)
                    {

                        post.TotalComments = ctx.BlogComments.Count(x => x.Post_Id == post.Id);
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

                ctx.Configuration.LazyLoadingEnabled = true;

                var res = ctx.BlogPosts.Include(x => x.Admin).Include(x => x.BlogComments.Select(x1 => x1.User)).Where(x=>x.Id==id).FirstOrDefault();


                CustomResponse<BlogPosts> response = new CustomResponse<BlogPosts>
                { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = res };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("SearchBlogs")]
        public async Task<IHttpActionResult> SearchBlogs(string BlogTitle, string CategoryType, string DateOfPosting)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    string conditions = string.Empty;

                    if (!String.IsNullOrEmpty(BlogTitle))
                        conditions += "AND BlogPosts.Title LIKE '%" + BlogTitle + "%'";

                    if (!String.IsNullOrEmpty(CategoryType))
                        conditions += "AND CategoryType='" + CategoryType + "'";

                    if (DateOfPosting != null)
                    {
                        conditions += "OR DateOfPosting='" + Convert.ToDateTime(DateOfPosting) + "'";
                    }



                    #region query
                    var query = @"select 
BlogPosts.Id,
BlogPosts.Title,
BlogPosts.CategoryType,
BlogPosts.DateOfPosting,
BlogPosts.Description,
BlogPosts.ImageUrl,
BlogPosts.is_popular,
BlogPosts.Title,
BlogPosts.Admin_ID,
BlogPosts.is_deleted,
Admins.Email
 from BlogPosts
 LEFT JOIN Admins
ON Admins.Id=BlogPosts.Admin_ID
Where BlogPosts.is_deleted=0 " + conditions + "";

                    #endregion


                    var Blogs = ctx.Database.SqlQuery<SearchBlogViewModel>(query).ToList();

                    return Ok(new CustomResponse<SearchBlogListViewModel> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchBlogListViewModel { BlogList = Blogs } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetEntityById")]
        public async Task<IHttpActionResult> GetEntityById(int Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var Blog = ctx.BlogPosts.FirstOrDefault(x => x.Id == Id && x.is_deleted == 0);
                    if (Blog == null)
                    {
                        return Ok(new CustomResponse<Error> { Message = ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Record not found" } });

                    }

                    return Ok(new CustomResponse<DAL.BlogPosts> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = Blog });



                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }



        [HttpGet]
        [Route("DeleteEntity")]
        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        public async Task<IHttpActionResult> DeleteEntity(int Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var Blog = ctx.BlogPosts.FirstOrDefault(x => x.Id == Id);
                    if (Blog != null)
                    {
                        Blog.is_deleted = 1;
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
