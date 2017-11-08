using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    public class BlogController : Controller
    {
        // GET: Dashboard/Blog
        public ActionResult Index(int? BlogId)
        {
            AddBlogViewModel model = new AddBlogViewModel();
            Request.RequestContext.HttpContext.Session.Remove("AddAdminImage");
            Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
            if (BlogId.HasValue)
            {
                var responseBlog = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Blog/GetEntityById", User, null, true, false, null,"Id=" + BlogId.Value));
                if (responseBlog == null || responseBlog is Error)
                {

                }
                else
                {
                    model.Blog = responseBlog.GetValue("Result").ToObject<BlogViewModel>();
                }
            }
            model.SetSharedData(User);
            return View(model);
        }
        [HttpPost]
        public JsonResult DeleteImageOnEdit()
        {
            return Json("Success");
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddBlogImage");
                Request.RequestContext.HttpContext.Session.Add("AddBlogImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddBlogImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        public ActionResult ManageBlogs()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        public ActionResult SearchBlog(SearchBlogModel returnModel)
        {
            //SearchBlogModel returnModel = new SearchBlogModel();
            if(!string.IsNullOrEmpty(returnModel.BlogTitle) || !string.IsNullOrEmpty(returnModel.CategoryType)){
                var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllStores", User, GetRequest: true));

                if (responseStores == null || responseStores is Error)
                {

                }
                else
                {
                    var Stores = responseStores.GetValue("Result").ToObject<List<StoreBindingModel>>();



                }

            }
           
            return PartialView("_SearchBlog", returnModel);
        }

        public ActionResult SearchBlogResults(SearchBlogModel model)
        {
            SearchBlogViewModel returnModel = new SearchBlogViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Blog/SearchBlogs", User, null, true, false, null, "BlogTitle=" + model.BlogTitle + "", "CategoryType=" + model.CategoryType, "DateOfPosting=" + model.DateOfPosting));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchBlogViewModel>();
            }
            
            returnModel.SetSharedData(User);
            return PartialView("_SearchBlogResults", returnModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AddBlogViewModel model)
        {
            model.SetSharedData(User);
            model.Blog.DateOfPosting = DateTime.Now;
            if (model.Blog.Id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
            }


            MultipartFormDataContent content;

            bool FileAttached = (Request.RequestContext.HttpContext.Session["AddBlogImage"] != null);
            bool ImageDeletedOnEdit = false;
            var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
            if (imgDeleteSessionValue != null)
            {
                ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
            }
            byte[] fileData = null;
            var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddBlogImage"];
            if (FileAttached)
            {
                using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                {

                    fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                }
            }

            ByteArrayContent fileContent;
            JObject response;

            bool firstCall = true;
            callAgain: content = new MultipartFormDataContent();
            if (FileAttached)
            {
                fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = ImageFile.FileName };
                content.Add(fileContent);
            }

            if (model.Blog.Id > 0)
                content.Add(new StringContent(model.Blog.Id.ToString()), "Id");

            if (model.Blog.User_ID==0)
                content.Add(new StringContent(model.Id.ToString()), "User_Id");

            content.Add(new StringContent(model.Blog.Title), "Title");
            content.Add(new StringContent(model.Blog.CategoryType), "CategoryType");
            content.Add(new StringContent(model.Blog.DateOfPosting.ToString()), "DateOfPosting");
            content.Add(new StringContent(model.Blog.Description), "Description");

            content.Add(new StringContent(model.Role.ToString()), "Role");
            
            content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");

            response = await ApiCall.CallApi("api/Blog/InsertBlog", User, isMultipart: true, multipartContent: content);

            if (firstCall && response.ToString().Contains("UnAuthorized"))
            {
                firstCall = false;
                goto callAgain;
            }
            else if (response.ToString().Contains("UnAuthorized"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
            }

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model.Blog = response.GetValue("Result").ToObject<BlogViewModel>();
                var claimIdentity = ((ClaimsIdentity)User.Identity);
                //if (model.Blog.Id == Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "AdminId").Value))
                //{
                //    User.AddUpdateClaim("FullName", model.Admin.FirstName + " " + model.Admin.LastName);
                //    User.AddUpdateClaim("ProfilePictureUrl", model.Admin.ImageUrl);
                //}
                model.SetSharedData(User);

                    if (model.Blog.Id > 0)
                        TempData["SuccessMessage"] = "The Blog been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The Blog has been added successfully.";
              
               

                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");
            }
        }

        public JsonResult DeleteBlog(int blogid)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Blog/DeleteEntity", User, null, true, false, null,"Id=" + blogid));
                if (response is Error)
                    return Json("An error has occurred, error code : 500", JsonRequestBehavior.AllowGet);
                else
                    return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}