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
using System.Web.UI;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
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
                Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
                Request.RequestContext.HttpContext.Session.Add("AddProductImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        public JsonResult FetchCategories(int storeId) // its a GET, not a POST
        {
            CategoryBindlingListModel returnModel = new CategoryBindlingListModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Categories/GetAllCategoriesByStoreId", User, GetRequest: true, parameters: "StoreId=" + storeId));

            var Storeresponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Shop/GetStoreById", User, GetRequest: true, parameters: "Id=" + storeId));
            var responseStore = Storeresponse.GetValue("Result").ToObject<StoreViewModel>();


            returnModel.responseCategories = response.GetValue("Result").ToObject<List<CategoryBindingModel>>();
            returnModel.BusinessType = responseStore.BusinessType;

            var tempCats = returnModel.responseCategories.ToList();

            foreach (var cat in returnModel.responseCategories)
            {
                cat.Name = cat.GetFormattedBreadCrumb(tempCats);
            }

            returnModel.responseCategories.Insert(0, new CategoryBindingModel { Id = 0, Name = "None" });

            return Json(returnModel, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FetchBeerUnits(int Type) // its a GET, not a POST
        {
            List<SizeBindingModel> Units = new List<SizeBindingModel>();
            var responseUnits = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Size/GetAllUnits", User, GetRequest: true, parameters: "Type=" + Type));
            if (responseUnits == null || responseUnits is Error)
            {
                return null;
            }else
            {
              Units = responseUnits.GetValue("Result").ToObject<List<SizeBindingModel>>();
            }

            //model.SizeUnits = Utility.GetUnits(User, "None",1);
            return Json(Units, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Index(int? ProductId)
        {
            Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
            Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
            AddProductViewModel model = new AddProductViewModel();

            model.SetSharedData(User);

            int initialStoreId = model.StoreId;
            //Providing StoresList

            model.StoreOptions = Utility.GetStoresOptions(User);
            model.SizeUnits = Utility.GetUnits(User);


            if (model.StoreOptions.Count() > 0)
            {
                initialStoreId = initialStoreId == 0 ? Convert.ToInt32((model.StoreOptions.Items as IEnumerable<StoreDropDownBindingModel>).First().Value) : initialStoreId;
            }
           
            if (ProductId.HasValue)
            {
                var responseProduct = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetEntityById", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Product, "Id=" + ProductId.Value));
                if (responseProduct == null || responseProduct is Error)
                    ;
                else
                {
                    model.Product = responseProduct.GetValue("Result").ToObject<ProductBindingModel>();
                    initialStoreId = model.Product.Store_Id;
                }
            }
            else
                model.Product.Store_Id = model.StoreId;

            //Providing CategoryList
            model.CategoryOptions = Utility.GetCategoryOptions(User, initialStoreId, "None");

            //model.WeightOptions = Utility.GetWeightOptions();

            //if (model.Product.WeightUnit == (int)WeightUnits.kg)
            //{
            //    model.Product.WeightInGrams = model.Product.WeightInKiloGrams;
            //}

            //return PartialView("_AddProduct", model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AddProductViewModel model)
        {
            try
            {
                model.Product.Description = model.Product.Description ?? "";
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                //if (model.Product.WeightUnit == (int)WeightUnits.kg)
                //{
                //    model.Product.WeightInKiloGrams = model.Product.WeightInGrams;
                //    model.Product.WeightInGrams = null;
                //}

                List<ProductSizeBindingModel> ProductSizes = new List<ProductSizeBindingModel>();

                MultipartFormDataContent content;

                bool FileAttached = (Request.RequestContext.HttpContext.Session["AddProductImage"] != null);
                bool ImageDeletedOnEdit = false;
                var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
                if (imgDeleteSessionValue != null)
                {
                    ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                }
                byte[] fileData = null;
                var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddProductImage"];
                if (FileAttached)
                {
                    using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                    {

                        fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                    }
                }
                else if (model.Product.Image == null || ImageDeletedOnEdit)
                {
                    //return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Please Choose an image to upload.");
                }

                ByteArrayContent fileContent;
                JObject response;

                //if (FileAttached)
                //{
                bool firstCall = true;
                callAgain: content = new MultipartFormDataContent();
                if (FileAttached)
                {
                    fileContent = new ByteArrayContent(fileData);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = ImageFile.FileName };
                    content.Add(fileContent);
                }
                if (model.Product.Id > 0)
                {
                    content.Add(new StringContent(model.Product.Id.ToString()), "Id");
                }
                if (model.Product.ProductSizes.Count > 0)
                {
                    var Sizes = JsonConvert.SerializeObject((model.Product.ProductSizes));
                    var bufferProductSizes = System.Text.Encoding.UTF8.GetBytes(Sizes);
                    var byteContentProductSizes = new ByteArrayContent(bufferProductSizes);
                    byteContentProductSizes.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    content.Add(byteContentProductSizes, "ProductSizes");
                }else
                {
                    content.Add(new StringContent(model.Product.Price.ToString()), "Price");
                }

                content.Add(new StringContent(model.Product.Name), "Name");
                content.Add(new StringContent(model.Product.Category_Id.ToString()), "Category_Id");
                content.Add(new StringContent(model.Product.Store_Id.ToString()), "Store_Id");
                content.Add(new StringContent(model.Product.Description), "Description");
                //content.Add(new StringContent(model.Product.WeightUnit.ToString()), "WeightUnit");

                //if (model.Product.WeightInGrams.HasValue)
                //{
                //    content.Add(new StringContent(Convert.ToString(model.Product.WeightInGrams)), "WeightInGrams");
                //}
                //else
                //{
                //    content.Add(new StringContent(Convert.ToString(model.Product.WeightInKiloGrams)), "WeightInKiloGrams");
                //}

                content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");
                response = await ApiCall.CallApi("api/Admin/AddProduct", User, isMultipart: true, multipartContent: content);
                if (firstCall && response.ToString().Contains("UnAuthorized"))
                {
                    firstCall = false;
                    goto callAgain;
                }
                else if (response.ToString().Contains("UnAuthorized"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
                }
                //}
                //else
                //    response = await ApiCall.CallApi("api/Admin/AddProduct", User, model.Product);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
                else
                {
                    if (model.Product.Id > 0)
                        TempData["SuccessMessage"] = "The product has been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The product has been added successfully.";

                    //return RedirectToAction("ManageProducts");
                    return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public ActionResult ManageProducts()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }
        
        public ActionResult SearchProduct()
        {
            SearchProductModel returnModel = new SearchProductModel();

            var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllStores", User, GetRequest: true));
            if (responseStores == null || responseStores is Error)
            {
            }
            else
            {
                var Stores = responseStores.GetValue("Result").ToObject<List<StoreBindingModel>>();
                IEnumerable<SelectListItem> selectList = from store in Stores
                                                         select new SelectListItem
                                                         {
                                                             Selected = false,
                                                             Text = store.BusinessName,
                                                             Value = store.Id.ToString()
                                                         };
                Stores.Insert(0, new StoreBindingModel { Id = 0, BusinessName = "All" });

                returnModel.StoreOptions = new SelectList(selectList);
            }

            returnModel.SetSharedData(User);
            //returnModel. returnModel.StoreId
            if (returnModel.Role == RoleTypes.SubAdmin)
            {
                returnModel.StoreId = (returnModel as BaseViewModel).StoreId;

            }



            return PartialView("_SearchProduct", returnModel);
        }

        public ActionResult SearchProductResults(SearchProductModel model)
        {
            SearchProductsViewModel returnModel = new SearchProductsViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchProducts", User, null, true, false, null, "ProductName=" + model.ProductName + "", "ProductPrice=" + model.ProductPrice, "CategoryName=" + model.CategoryName + "", "StoreId=" + model.StoreId));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchProductsViewModel>();
            }
            returnModel.SetSharedData(User);
            return PartialView("_SearchProductResults", returnModel);
        }

        public JsonResult DeleteProduct(int ProductId)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)BasketEntityTypes.Product, "Id=" + ProductId));
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
        public JsonResult GetCategoryName(int Category_Id)
        {
            try
            {
                var  ParentCategoryName = string.Empty;
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Products/GetCategoryName", User, null, true, false, null, parameters: "Category_Id=" + Category_Id));
                if (response is Error)
                    return Json("An error has occurred, error code : 500", JsonRequestBehavior.AllowGet);
                else
                {
                    ParentCategoryName = response.GetValue("Result").ToObject<string>();
                }


                return Json(ParentCategoryName, JsonRequestBehavior.AllowGet);
                    //return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult DemoFile()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");
            dt.Columns.Add("Price");

            //Column addition ends here

            dt.Rows.Add("", "", "");

            System.Web.UI.WebControls.GridView grdView = new System.Web.UI.WebControls.GridView();
            grdView.DataSource = dt;

            grdView.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            grdView.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            Global.sharedDataModel.SetSharedData(User);
            return View("ManageProducts", Global.sharedDataModel);
        }


        public ActionResult ExportProduct()
        {
            try
            {
                
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/File/ExportProducts", User, null,false, false, null, "EntityType=" + (int)BasketEntityTypes.Product));
                var resp = response.GetValue("Result").ToObject<string>();
                if (response is Error)
                    return Json("An error has occurred, error code : 500", JsonRequestBehavior.AllowGet);
                else
                    //var resp = response.GetValue("Result").ToObject<string>();
                    return Json(resp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // for file import export excel

        public async Task<ActionResult> ImportFile()
        {
            try
            {
                MultipartFormDataContent content;

                bool FileAttached = (Request.RequestContext.HttpContext.Session["AddProductFile"] != null);
                bool ImageDeletedOnEdit = false;
                var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["FileDeletedOnEdit"];
                if (imgDeleteSessionValue != null)
                {
                    ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                }
                byte[] fileData = null;
                var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddProductFile"];
                if (FileAttached)
                {
                    using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                    {

                        fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                    }
                }
                

                ByteArrayContent fileContent;
                JObject response;

                //if (FileAttached)
                //{
                bool firstCall = true;
                callAgain: content = new MultipartFormDataContent();
                if (FileAttached)
                {
                    fileContent = new ByteArrayContent(fileData);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = ImageFile.FileName };
                    content.Add(fileContent);
                }
                

                content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "FileDeletedOnEdit");
                response = await ApiCall.CallApi("api/Admin/AddProduct", User, isMultipart: true, multipartContent: content);
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
                        TempData["SuccessMessage"] = "Products saved successfully.";

                        return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        [HttpPost]
        public JsonResult DeleteFileOnEdit()
        {
            return Json("Success");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddProductFile");
                Request.RequestContext.HttpContext.Session.Add("AddProductFile", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("FileDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("FileDeletedOnEdit", false);



                try
                {
                    MultipartFormDataContent content;

                    bool FileAttached = (Request.RequestContext.HttpContext.Session["AddProductFile"] != null);
                    bool ImageDeletedOnEdit = false;
                    var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["FileDeletedOnEdit"];
                    if (imgDeleteSessionValue != null)
                    {
                        ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                    }
                    byte[] fileData = null;
                    var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddProductFile"];
                    if (FileAttached)
                    {
                        using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                        {

                            fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                        }
                    }


                    ByteArrayContent fileContent;
                    JObject response;

                    //if (FileAttached)
                    //{
                    bool firstCall = true;
                    callAgain: content = new MultipartFormDataContent();
                    if (FileAttached)
                    {
                        fileContent = new ByteArrayContent(fileData);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = ImageFile.FileName };
                        content.Add(fileContent);
                    }


                    content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "FileDeletedOnEdit");
                    response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/File/ImportProducts", User,GetRequest:false, isMultipart: true, multipartContent: content));
                    
                    if (response is Error || response==null)
                    {
                        //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                        return Json(new { success = false, responseText = "Error" }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        if (firstCall && response.ToString().Contains("UnAuthorized"))
                        {
                            firstCall = false;
                            goto callAgain;
                        }
                        else if (response.ToString().Contains("UnAuthorized"))
                        {
                            return Json(new { success = false, responseText = "UnAuthorizeds" }, JsonRequestBehavior.DenyGet);
                        }
                        return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
          
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteFile()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddProductFile");
            Request.RequestContext.HttpContext.Session.Add("FileDeletedOnEdit", true);
            return Json("Session Cleared");
        }


    }
}