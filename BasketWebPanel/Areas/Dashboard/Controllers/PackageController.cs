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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class PackageController : Controller
    {
        public ActionResult GetStoreProducts(SearchProductModel model)
        {
            PackageProductsViewModel returnModel = new PackageProductsViewModel();
            JObject response;
            if (model.PackageId.HasValue == false || (model.PackageId.HasValue == true && model.PackageId.Value == 0))
            {
                response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchProducts", User, null, true, false, null, "ProductName=" + model.ProductName + "", "ProductPrice=" + model.ProductPrice, "CategoryName=" + model.CategoryName + "", "StoreId=" + model.StoreId));
                if (response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                else
                    returnModel.Products = response.GetValue("Result").SelectToken("Products").ToObject<List<PackageProductViewModel>>();
            }
            else
            {

                response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Packages/GetPackageProducts", User, null, true, false, null, "PackageId=" + model.PackageId, "StoreId=" + model.StoreId));

                if (response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                else
                    returnModel.Products = response.GetValue("Result").SelectToken("Products").ToObject<List<PackageProductViewModel>>();
            }

            return PartialView("_PackageProducts", returnModel);
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
                Request.RequestContext.HttpContext.Session.Remove("AddPackageImage");
                Request.RequestContext.HttpContext.Session.Add("AddPackageImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddPackageImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        public ActionResult Index(int? PackageId)
        {
            Request.RequestContext.HttpContext.Session.Remove("AddPackageImage");
            Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");

            AddPackageViewModel model = new AddPackageViewModel();

            model.SetSharedData(User);
            int initialStoreId = model.StoreId;
            //Providing StoresList
            model.StoreOptions = Utility.GetStoresOptions(User);
            initialStoreId = initialStoreId == 0 ? Convert.ToInt32((model.StoreOptions.Items as IEnumerable<SelectListItem>).First().Value) : initialStoreId;

            //Edit Scenario
            if (PackageId.HasValue)
            {
                var responseProduct = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetEntityById", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Package, "Id=" + PackageId.Value));
                if (responseProduct == null || responseProduct is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                else
                {
                    model.Package = responseProduct.GetValue("Result").ToObject<PackageBindingModel>();
                    initialStoreId = model.Package.Store_Id;
                }
            }
            else
                model.Package.Store_Id = model.StoreId;
            

            return View(model);

            //var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchProducts", User, null, true, false, null, "ProductName=" + string.Empty + "", "ProductPrice=null", "CategoryName=" + string.Empty + "", "StoreId=" + initialStoreId));

            //if (response is Error)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            //}
            //else
            //{
            //    model.Products = response.GetValue("Result").SelectToken("Products").ToObject<List<PackageProductViewModel>>();
            //}

            //model.Products = response.GetValue("Result").SelectToken("Products").ToObject<List<PackageProductViewModel>>();

        }

        [HttpPost]
        public async Task<ActionResult> Index(PackageBindingModel Package, List<PackageProductViewModel> Products)
        {
            try
            {
                AddPackageViewModel model = new AddPackageViewModel();

                model.Package = Package;
                model.Products = Products;

                if (model.Products == null)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Please select products to add");

                var package_Products = model.Products.Where(x => x.IsChecked == true).ToList();

                model.Package.Description = model.Package.Description ?? "";

                if (package_Products.Count == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Please select products to add");
                }
                else if (package_Products.Count == 1 && package_Products.First().Qty == 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Quantity should be greater than 1 for single product in package");
                }

                MultipartFormDataContent content;

                bool FileAttached = (Request.RequestContext.HttpContext.Session["AddPackageImage"] != null);
                bool ImageDeletedOnEdit = false;
                var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
                if (imgDeleteSessionValue != null)
                {
                    ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                }
                byte[] fileData = null;
                var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddPackageImage"];
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
                if (model.Package.Id > 0)
                {
                    content.Add(new StringContent(model.Package.Id.ToString()), "Id");
                }
                content.Add(new StringContent(model.Package.Name), "Name");
                content.Add(new StringContent(model.Package.Price.ToString()), "Price");
                content.Add(new StringContent(model.Package.Store_Id.ToString()), "Store_Id");
                content.Add(new StringContent(model.Package.Description), "Description");
                content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");

                var packageProducts = JsonConvert.SerializeObject(package_Products);


                var buffer = System.Text.Encoding.UTF8.GetBytes(packageProducts);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Add(byteContent, "package_products");


                response = await ApiCall.CallApi("api/Admin/AddPackage", User, isMultipart: true, multipartContent: content);

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
                    if (model.Package.Id > 0)
                        TempData["SuccessMessage"] = "The package has been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The package has been added successfully.";

                    //return RedirectToAction("ManageProducts");
                    return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex));
            }
        }


        public ActionResult ManagePackages()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }


        public ActionResult SearchPackage()
        {
            SearchPackageModel returnModel = new SearchPackageModel();

            returnModel.StoreOptions = Utility.GetStoresOptions(User, "All");

            returnModel.SetSharedData(User);

            if (returnModel.Role == RoleTypes.SubAdmin)
            {
                returnModel.StoreId = (returnModel as BaseViewModel).StoreId;
            }

            return PartialView("_SearchPackages", returnModel);
        }

        public ActionResult SearchPackageResults(SearchPackageModel model)
        {
            SearchPackageListViewModel returnModel = new SearchPackageListViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchPackages", User, null, true, false, null, "PackageName=" + model.PackageName, "StoreId=" + model.StoreId));
            if (response == null || response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchPackageListViewModel>();
            }

            returnModel.SetSharedData(User);
            return PartialView("_SearchPackagesResult", returnModel);
        }

        public JsonResult DeletePackage(int PackageId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Package, "Id=" + PackageId));
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