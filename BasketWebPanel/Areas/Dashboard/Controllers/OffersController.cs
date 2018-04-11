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
    public class OffersController : Controller
    {
        public ActionResult GetStoreProducts(SearchOfferBindingModel model)
        {
            OfferProductAndPackagesViewModel returnModel = new OfferProductAndPackagesViewModel();
            JObject response;
            if (model.OfferId.HasValue == false || (model.OfferId.HasValue == true && model.OfferId.Value == 0))
            {
                response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchProducts", User, null, true, false, null, "ProductName=" + model.ProductName + "", "ProductPrice=" + model.ProductPrice, "CategoryName=" + model.CategoryName + "", "StoreId=" + model.StoreId));
                if (response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                else
                    returnModel.Products = response.GetValue("Result").SelectToken("Products").ToObject<List<OfferProductViewModel>>();

                response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchPackages", User, null, true, false, null, "PackageName=" + model.PackageName, "StoreId=" + model.StoreId));

                if (response == null || response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                else
                    returnModel.Packages = response.GetValue("Result").ToObject<OfferProductAndPackagesViewModel>().Packages;
            }
            else
            {

                response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Deals/GetOfferProductsAndPackages", User, null, true, false, null, "OfferId=" + model.OfferId, "StoreId=" + model.StoreId));

                if (response is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                else
                {
                    returnModel = response.GetValue("Result").ToObject<OfferProductAndPackagesViewModel>();
                }

            }

            return PartialView("_OfferProducts", returnModel);
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
                Request.RequestContext.HttpContext.Session.Remove("AddOfferImage");
                Request.RequestContext.HttpContext.Session.Add("AddOfferImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddOfferImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }
        // GET: Dashboard/Offers
        public ActionResult Index(int? OfferId)
        {
            Request.RequestContext.HttpContext.Session.Remove("AddOfferImage");
            Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
            AddOfferViewModel model = new AddOfferViewModel();
            model.SetSharedData(User);

            int initialStoreId = model.StoreId;
            model.StoreOptions = Utility.GetStoresOptions(User);

            if (model.StoreOptions.Count() > 0)
                initialStoreId = initialStoreId == 0 ? Convert.ToInt32((model.StoreOptions.Items as IEnumerable<StoreDropDownBindingModel>).First().Value) : initialStoreId;
                //initialStoreId = initialStoreId == 0 ? Convert.ToInt32((model.StoreOptions.Items as IEnumerable<SelectListItem>).First().Value) : initialStoreId;

            //Edit Scenario
            if (OfferId.HasValue)
            {
                var responseProduct = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetEntityById", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Offer, "Id=" + OfferId.Value));
                if (responseProduct == null || responseProduct is Error)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                else
                {
                    model.Offer = responseProduct.GetValue("Result").ToObject<OfferViewModel>();
                    dynamic getDates = JObject.Parse(responseProduct.SelectToken("Result").ToString());
                    model.Offer.ValidFrom = getDates.ValidFrom;
                    model.Offer.ValidTo = getDates.ValidUpto;
                    initialStoreId = model.Offer.Store_Id;
                }
            }
            else
            {
                model.Offer.ValidFrom = DateTime.Now;
                model.Offer.ValidTo = DateTime.Now;
                model.Offer.Store_Id = model.StoreId;
            }


            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(OfferViewModel Offer, List<OfferProductViewModel> Products, List<OfferPackageViewModel> Packages)
        {
            try
            {
                AddOfferViewModel model = new AddOfferViewModel();
                model.Offer = Offer;
                model.Products = Products;
                model.Packages = Packages;
                model.Offer.Description = model.Offer.Description ?? "";

                if (!ModelState.IsValid)
                {
                    return View(ModelState);
                }

                if (model.Products == null && model.Packages == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Please select products or packages to add");
                }

                List<OfferProductViewModel> offer_products = new List<OfferProductViewModel>();
                List<OfferPackageViewModel> offer_packages = new List<OfferPackageViewModel>();

                if (model.Products != null)
                    offer_products = model.Products.Where(x => x.IsChecked).ToList();

                if (model.Packages != null)
                    offer_packages = model.Packages.Where(x => x.IsChecked).ToList();

                if (offer_packages.Count == 0 && offer_products.Count == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Please select products or packages to add");
                }

                MultipartFormDataContent content;

                bool FileAttached = (Request.RequestContext.HttpContext.Session["AddOfferImage"] != null);
                bool ImageDeletedOnEdit = false;
                var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
                if (imgDeleteSessionValue != null)
                {
                    ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                }

                byte[] fileData = null;
                var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddOfferImage"];
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

                if (model.Offer.Id > 0)
                {
                    content.Add(new StringContent(model.Offer.Id.ToString()), "Id");
                }

                content.Add(new StringContent(model.Offer.Name), "Name");
                content.Add(new StringContent(model.Offer.Store_Id.ToString()), "Store_Id");
                content.Add(new StringContent(model.Offer.ValidFrom.ToString("dd/MM/yyyy hh:mm:ss tt")), "ValidFrom");
                content.Add(new StringContent(model.Offer.ValidTo.ToString("dd/MM/yyyy hh:mm:ss tt")), "ValidTo");
                content.Add(new StringContent(model.Offer.Description), "Description");
                content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");

                var offerProducts = JsonConvert.SerializeObject(offer_products);

                var offerPackages = JsonConvert.SerializeObject(offer_packages);

                var bufferProduct = System.Text.Encoding.UTF8.GetBytes(offerProducts);
                var byteContentProduct = new ByteArrayContent(bufferProduct);
                byteContentProduct.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Add(byteContentProduct, "offer_products");

                var bufferPackages = System.Text.Encoding.UTF8.GetBytes(offerPackages);
                var byteContentPackages = new ByteArrayContent(bufferPackages);
                byteContentPackages.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Add(byteContentPackages, "offer_packages");

                response = await ApiCall.CallApi("api/Admin/AddOffer", User, isMultipart: true, multipartContent: content);

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
                    if (model.Offer.Id > 0)
                        TempData["SuccessMessage"] = "The offer has been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The offer has been added successfully.";

                    return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }

        public ActionResult ManageOffers()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        public ActionResult SearchOffer()
        {
            SearchOfferModel returnModel = new SearchOfferModel();

            returnModel.StoreOptions = Utility.GetStoresOptions(User, "All");

            returnModel.SetSharedData(User);

            if (returnModel.Role == RoleTypes.SubAdmin)
            {
                returnModel.StoreId = (returnModel as BaseViewModel).StoreId;
            }

            return PartialView("_SearchOffer", returnModel);
        }

        public ActionResult SearchOfferResults(SearchOfferModel model)
        {
            SearchOfferListViewModel returnModel = new SearchOfferListViewModel();

            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchOffers", User, null, true, false, null, "OfferName=" + model.OfferName, "StoreId=" + model.StoreId));
            if (response == null || response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchOfferListViewModel>();
            }

            returnModel.SetSharedData(User);
            return PartialView("_SearchOfferResults", returnModel);
        }

        public JsonResult DeleteOffer(int OfferId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Offer, "Id=" + OfferId));
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