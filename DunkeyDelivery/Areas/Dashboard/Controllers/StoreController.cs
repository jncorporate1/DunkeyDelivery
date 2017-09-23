using DunkeyDelivery;
using DunkeyDelivery.Areas.Dashboard.Models;
using DunkeyDelivery.BindingModels;
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

namespace DunkeyDelivery.Areas.Dashboard.Controllers
{
    public class StoreController : Controller
    {
        // GET: Dashboard/Store
        public ActionResult Index()
        {
            var model = new ShopViewModel();
            model.SetSharedData(User);
            return View(model);
        }
        public ActionResult AddStore()
        {
            var model = new ShopViewModel();
            model.SetSharedData(User);
            return PartialView("_AddStore", model);
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddStoreImage");
                Request.RequestContext.HttpContext.Session.Add("AddStoreImage", Request.Files[0]);
            }
            return Json("Success");
        }


     
        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddStoreImage");
            return Json("Session Cleared");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ShopViewModel model, string returnUrl)
        {

            #region
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            //var response = await ApiCall<ShopViewModel>.CallApi("api/Shop/Create", model);

            //if (response is Error)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            //}
            //else
            //{
            //    //TempData["notice"] = "* Store added successfully";
            //    //return RedirectToAction("Index");
            //    return RedirectToAction("AddStore");

            //}
            #endregion


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            MultipartFormDataContent content;
            bool FileAttached = (Request.RequestContext.HttpContext.Session["AddStoreImage"] != null);
            byte[] fileData = null;
            var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddStoreImage"];
            if (FileAttached)
                using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                {
                    try
                    {
                        fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                    }catch(Exception ex)
                    {
                        throw (ex);
                    }
                }

            ByteArrayContent fileContent;
            JObject response;

            if (FileAttached)
            {
                content = new MultipartFormDataContent();
                fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = ImageFile.FileName };
                content.Add(fileContent);
                content.Add(new StringContent(model.BusinessName), "BusinessName");
                content.Add(new StringContent(model.BusinessType), "BusinessType");
                content.Add(new StringContent(model.Latitude.ToString()), "Latitude");
                content.Add(new StringContent(model.Longitude.ToString()), "Longitude");
                response = await ApiCall<ShopViewModel>.CallApiAsMultipart("api/Shop/CreateWithImage", content);
            }
            else
                response = await ApiCall<ShopViewModel>.CallApi("api/Shop/Create", model);

        

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                var a = response.GetValue("Result").ToObject<ShopViewModel>();
                return RedirectToAction("AddStore");
            }


        }




    }
}