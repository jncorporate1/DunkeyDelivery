using BasketWebPanel;
using BasketWebPanel.Areas.Dashboard.Models;
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
    [Authorize]
    public class CategoriesController : Controller
    {
        // GET: Dashboard/Categories
        //public ActionResult Index()
        //{
        //    //return RedirectToAction("AddCategory");
        //    Global.sharedDataModel.SetSharedData(User);
        //    return View(Global.sharedDataModel);
        //}

        public ActionResult Manage()
        {
            return View();
        }

        public JsonResult FetchCategories(int storeId) // its a GET, not a POST
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Categories/GetAllCategoriesByStoreId", User, GetRequest: true, parameters: "StoreId=" + storeId));
            var responseCategories = response.GetValue("Result").ToObject<List<CategoryBindingModel>>();
            var tempCats = responseCategories.ToList();

            foreach (var cat in responseCategories)
            {
                cat.Name = cat.GetFormattedBreadCrumb(tempCats);
            }

            responseCategories.Insert(0, new CategoryBindingModel { Id = 0, Name = "None" });

            return Json(responseCategories, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int? CategoryId)
        {
            try
            {
                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Remove("AddCategoryImage");
                AddCategoryViewModel model = new AddCategoryViewModel();
                model.SetSharedData(User);

                var initialStoreId = model.StoreId;

                model.StoreOptions = Utility.GetStoresOptions(User);

                initialStoreId = initialStoreId == 0 ? Convert.ToInt32((model.StoreOptions.Items as IEnumerable<SelectListItem>).First().Value) : initialStoreId;

                if (CategoryId.HasValue)
                {
                    var responseProduct = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetEntityById", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Category, "Id=" + CategoryId.Value));
                    if (responseProduct == null || responseProduct is Error)
                        ;
                    else
                    {
                        model.Category = responseProduct.GetValue("Result").ToObject<CategoryBindingModel>();
                        initialStoreId = model.Category.Store_Id;
                        model.ParentCategoryOptions = Utility.GetCategoryOptions(User, initialStoreId, "None", model.Category.Id);
                    }
                }
                else
                {
                    model.ParentCategoryOptions = Utility.GetCategoryOptions(User, initialStoreId, "None");
                    model.Category.Store_Id = model.StoreId;
                }

                model.SetSharedData(User);

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AddCategoryViewModel model)
        {
            try
            {
                model.Category.Description = model.Category.Description ?? "";
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                MultipartFormDataContent content;

                bool FileAttached = (Request.RequestContext.HttpContext.Session["AddCategoryImage"] != null);
                bool ImageDeletedOnEdit = false;
                var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
                if (imgDeleteSessionValue != null)
                {
                    ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                }
                byte[] fileData = null;
                var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddCategoryImage"];
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
                if (model.Category.Id > 0)
                {
                    content.Add(new StringContent(model.Category.Id.ToString()), "Id");
                }
                content.Add(new StringContent(model.Category.Name), "Name");
                content.Add(new StringContent(model.Category.Store_Id.ToString()), "Store_Id");
                content.Add(new StringContent(model.Category.Description), "Description");
                content.Add(new StringContent(Convert.ToString(model.Category.ParentCategoryId)), "ParentCategoryId");
                content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");
                response = await ApiCall.CallApi("api/Admin/AddCategory", User, isMultipart: true, multipartContent: content);
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
                    if (model.Category.Id > 0)
                        TempData["SuccessMessage"] = "The category has been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The category has been added successfully.";
                    
                    return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult ManageCategories()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddCategoryImage");
                Request.RequestContext.HttpContext.Session.Add("AddCategoryImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddCategoryImage");

            Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        public ActionResult SearchCategory()
        {
            SearchCategoryModel returnModel = new SearchCategoryModel();

            returnModel.StoreOptions = Utility.GetStoresOptions(User, "All");

            returnModel.SetSharedData(User);

            if (returnModel.Role == RoleTypes.SubAdmin)
            {
                returnModel.StoreId = (returnModel as BaseViewModel).StoreId;
            }

            return PartialView("_SearchCategory", returnModel);
        }

        public ActionResult SearchCategoryResults(SearchCategoryModel model)
        {
            SearchCategoriesViewModel returnModel = new SearchCategoriesViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchCategories", User, null, true, false, null, "CategoryName=" + model.CategoryName, "StoreId=" + model.StoreId));
            if (response == null || response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchCategoriesViewModel>();
            }
            var tempCats = returnModel.Categories.ToList();
            foreach (var cat in returnModel.Categories)
            {
                cat.Name = cat.GetFormattedBreadCrumb(tempCats);
            }
            returnModel.SetSharedData(User);
            return PartialView("_SearchCategoryResults", returnModel);
        }

        public JsonResult DeleteCategory(int CategoryId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Category, "Id=" + CategoryId));
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