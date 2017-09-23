using DunkeyDelivery.Areas.Dashboard.Models;
using DunkeyDelivery.BindingModels;
using DunkeyDelivery.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.Dashboard.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Dashboard/Categories
        public ActionResult Index()
        {


            return View(new AddCategoryViewModel());
        }
        public ActionResult Manage()

        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create(AddCategoryViewModel model)
        {

            model.Status = 1;
            if (!ModelState.IsValid)
            {
                return View("Index",model);
            }


            var response = await ApiCall<AddCategoryViewModel>.CallApi("api/Categories/Create", model);


            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);

            }


            else
            {

                return RedirectToAction("AddCategory");
            }





        }
        public ActionResult AddCategory()
        {
            try
            {
                AddCategoryViewModel model = new AddCategoryViewModel();
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall<AddCategoryViewModel>.CallApi("api/Shop/GetStoreCategories?Store_id=1", null,false));

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
                    model.ParentCategoryOptions = new SelectList(selectList);
                }

                //Providing StoresList
                var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall<AddCategoryViewModel>.CallApi("api/Shop/GetAllStores", null,false));
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
                model.SetSharedData(User);
                return View("Index", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public JsonResult FetchCategories(int storeId) // its a GET, not a POST
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall<AddCategoryViewModel>.CallApi("api/Shop/GetStoreCategories?Store_id=" + storeId, null,false));
            var responseCategories = response.GetValue("Result").ToObject<List<CategoryBindingModel>>();
            var tempCats = responseCategories.ToList();

            foreach (var cat in responseCategories)
            {
                cat.Name = cat.GetFormattedBreadCrumb(tempCats);
            }

            responseCategories.Insert(0, new CategoryBindingModel { Id = 0, Name = "None" });

            return Json(responseCategories, JsonRequestBehavior.AllowGet);
        }
       

    }
}