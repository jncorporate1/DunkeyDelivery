using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json;
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
    public class StoresController : Controller
    {
        public ActionResult Index(int? StoreId)
        {
            Request.RequestContext.HttpContext.Session.Remove("AddStoreImage");
            Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
            AddStoreViewModel model = new AddStoreViewModel();
            model.SetSharedData(User);
            if (model.Role == RoleTypes.SubAdmin)
            {
                StoreId = model.StoreId;
            }
            if (StoreId.HasValue)
            {
                var responseStore = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetEntityById", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Store, "Id=" + StoreId.Value));
                if (responseStore == null || responseStore is Error)
                    ;
                else
                {
                    model.Store = responseStore.GetValue("Result").ToObject<StoreViewModel>();
                    model.Store.StoreDeliveryHours = model.Store.StoreDeliveryHours ?? new StoreDeliveryHoursViewModel();
                }
            }
           
            return View(model);
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddStoreImage");
                Request.RequestContext.HttpContext.Session.Add("AddStoreImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddStoreImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AddStoreViewModel model)
        {
            model.Store.Description = model.Store.Description ?? "";
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            MultipartFormDataContent content;
            bool FileAttached = (Request.RequestContext.HttpContext.Session["AddStoreImage"] != null);
            bool ImageDeletedOnEdit = false;
            var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
            if (imgDeleteSessionValue != null)
            {
                ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
            }
            byte[] fileData = null;
            var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddStoreImage"];
            if (FileAttached)
                using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
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
            if (model.Store.Id > 0)
            {
                content.Add(new StringContent(model.Store.Id.ToString()), "Id");
            }
            content.Add(new StringContent(model.Store.Name), "StoreName");
            content.Add(new StringContent(model.Store.Latitude.ToString()), "Lat");
            content.Add(new StringContent(model.Store.Longitude.ToString()), "Long");
            content.Add(new StringContent(model.Store.Open_From.ToString()), "Open_From");
            content.Add(new StringContent(model.Store.Open_To.ToString()), "Open_To");
            content.Add(new StringContent(Convert.ToString(model.Store.Description)), "Description");
            content.Add(new StringContent(Convert.ToString(model.Store.Address)), "Address");
            content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");
            var packageProducts = JsonConvert.SerializeObject(model.Store.StoreDeliveryHours);


            var buffer = System.Text.Encoding.UTF8.GetBytes(packageProducts);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Add(byteContent, "StoreDeliveryHours");

            response = await ApiCall.CallApi("api/Admin/AddStore", User, isMultipart: true, multipartContent: content);
            if (firstCall && Convert.ToString(response).Contains("UnAuthorized"))
            {
                firstCall = false;
                goto callAgain;
            }
            else if (Convert.ToString(response).Contains("UnAuthorized"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
            }

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model.SetSharedData(User);
                if (model.Role == RoleTypes.SuperAdmin)
                {
                    if (model.Store.Id > 0)
                        TempData["SuccessMessage"] = "The store has been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The store has been added successfully.";
                }
                
                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ManageStores()
        {
            StoresListViewModel model = new StoresListViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllStores", User, GetRequest: true));
            if (response == null || response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model.Stores = response.GetValue("Result").ToObject<List<StoreViewModel>>();
            }

            model.SetSharedData(User);

            return View(model);
        }


        public ActionResult SearchStores()
        {
            SearchPackageModel returnModel = new SearchPackageModel();

            returnModel.StoreOptions = Utility.GetStoresOptions(User, "All");

            return PartialView("_SearchStores", returnModel);
        }

        public ActionResult SearchStoreResults()
        {
            StoresListViewModel model = new StoresListViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllStores", User, GetRequest: true));
            if (response == null || response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model.Stores = response.GetValue("Result").ToObject<List<StoreViewModel>>();
            }

            model.SetSharedData(User);

            return PartialView("_SearchPackagesResult", model);
        }

        public JsonResult DeleteStore(int StoreId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Store, "Id=" + StoreId));
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
