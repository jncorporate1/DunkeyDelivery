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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace DunkeyDelivery.Areas.Dashboard.Controllers
{
    public class ProductController : Controller
    {

        HttpPostedFileBase productImage;

       
        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
                Request.RequestContext.HttpContext.Session.Add("AddProductImage", Request.Files[0]);
            }
            return Json("Success");
        }

        public ActionResult AddProduct()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
            AddProductViewModel model = new AddProductViewModel();

            //Providing StoresList
            var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall<AddProductViewModel>.CallApi("api/Shop/GetAllStores",null,false));
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
                model.StoreOptions = new SelectList(selectList);
            }

            //Providing CategoryList
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall<AddCategoryViewModel>.CallApi("api/Shop/GetStoreCategories?Store_id=1",null,false));

            if (response == null || response is Error)
            {

            }
            else
            {
                //Providing CategoriesList
                var responseCategories = response.GetValue("Result").ToObject<List<CategoryBindingModel>>();
                var tempCats = responseCategories.ToList();
                foreach (var cat in responseCategories)
                {
                    cat.Name = cat.GetFormattedBreadCrumb(tempCats);
                }

                responseCategories.Insert(0, new CategoryBindingModel { Id = 0, Name = "None" });
                IEnumerable<SelectListItem> selectList = from cat in responseCategories
                                                         select new SelectListItem
                                                         {
                                                             Selected = false,
                                                             Text = cat.Name,
                                                             Value = cat.Id.ToString()
                                                         };
                model.CategoryOptions = new SelectList(selectList);
            }
            model.SetSharedData(User);
            return PartialView("_AddProduct", model);
        }
        // GET: Dashboard/Product
        public ActionResult Index()
        {
            var model = new CategoryProductViewModel();
            model.SetSharedData(User);
            return View(model);
        }

        public JsonResult FetchCategories(int storeId) // its a GET, not a POST
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall<AddCategoryViewModel>.CallApi("api/Shop/GetStoreCategories?Store_id="+ storeId,null,false));
            var responseCategories = response.GetValue("Result").ToObject<List<CategoryBindingModel>>();
            var tempCats = responseCategories.ToList();

            

            responseCategories.Insert(0, new CategoryBindingModel { Id = 0, Name = "None" });

            return Json(responseCategories, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create(AddProductViewModel model, string returnUrl)
        {

            try
            {
                
                model.Description = model.Description ?? "";
               
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                MultipartFormDataContent content;
                bool FileAttached = (Request.RequestContext.HttpContext.Session["AddProductImage"] != null);
                byte[] fileData = null;
                var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddProductImage"];
                if (FileAttached)
                {
                    using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                    {

                        fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
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
                    content.Add(new StringContent(model.Name), "Name");
                    content.Add(new StringContent(model.Price), "Price");
                    content.Add(new StringContent(model.Category_Id.ToString()), "CatId");
                    content.Add(new StringContent(model.Store_Id.ToString()), "StoreId");
                    content.Add(new StringContent(model.Description), "Description");
                    response = await ApiCall<AddProductViewModel>.CallApiAsMultipart("api/Products/AddProductWithImage", content);
                }
                else
                    response = await ApiCall<AddProductViewModel>.CallApi("api/Products/Create", model);

                //var response = await ApiCall<AddProductViewModel>.CallApi("api/Admin/AddProductWithImage", model);
                //var response = await ApiCall<AddProductViewModel>.CallApiAsMultipart("api/Admin/AddProductWithImage", content);

                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
                else
                {
                    var a = response.GetValue("Result").ToObject<AddProductViewModel>();
                    return RedirectToAction("AddProduct");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }




        }
        

    }
}