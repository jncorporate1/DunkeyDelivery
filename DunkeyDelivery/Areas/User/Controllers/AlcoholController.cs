using DunkeyDelivery.Areas.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class AlcoholController : Controller
    {
        // GET: User/Alcohol
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Categories()
        {
            return View("StoreCategories");
        }
        //public ActionResult SubCategories()
        //{
        //    return View("SubCategories");
        //}
        public ActionResult Products()
        {
            return View("Products");
        }
        public ActionResult StoreCategories(AlcoholViewModel model)
        {

            return PartialView("_Categories",model.categoryViewModel );
        }
        public async Task<ActionResult> subcategories(int Category_id,string Name,int Store_id)
        {

            
            var cat = await ApiCall<List<CategoryViewModel>>.CallApi("api/Categories/GetSubCategories?Category_id=" + Category_id + "", null, false);
           

            var subCategories = cat.GetValue("Result").ToObject<List<CategoryViewModel>>();
            
            if (subCategories.Count == 0)
            {
                var Shop = await ApiCall<StoreViewModel>.CallApi("api/Shop/GetStoreById?Id=" + Store_id + "", null, false);

                var product = await ApiCall<AlcoholProductVM>.CallApi("api/Products/GetCategoryProducts?Category_Id=" + Category_id + "", null, false);
                var productResponse = product.GetValue("Result").ToObject<AlcoholProductVM>();
                productResponse.shopViewModel = Shop.GetValue("Result").ToObject<StoreViewModel>();

                productResponse.Name = Name;
                return PartialView("_products", productResponse);

            }
            else
            {
                CategorySubCategoryVM response = new CategorySubCategoryVM();
                response.Name = Name;
                response.CategoryViewModel = subCategories;
               return PartialView("_categories", response); 
            }
        }

        public async Task<ActionResult> GetProducts(short Category_id)
        {

            var cat = await ApiCall<List<CategoryViewModel>>.CallApi("api/Categories/GetSubCategories?Category_id=" + Category_id + "", null, false);

            var subCategories = cat.GetValue("Result").ToObject<List<CategoryViewModel>>();
            return PartialView("_Categories", subCategories);
        }



        [HttpGet]
        public async Task<ActionResult> SearchByName(string search_string, string CategoryType,int Store_Id)
        {
            AlcoholProductVM responseResult = new AlcoholProductVM();
            var response = await ApiCall<AlcoholProductVM>.CallApi("api/Products/GetAlcoholStores?search_string=" + search_string + "&CategoryType=" + CategoryType+ "&Store_Id="+ Store_Id, null, false);
            var Shop = await ApiCall<StoreViewModel>.CallApi("api/Shop/GetStoreById?Id=" + Store_Id + "", null, false);


           
            responseResult = response.GetValue("Result").ToObject<AlcoholProductVM>();
            responseResult.shopViewModel = Shop.GetValue("Result").ToObject<StoreViewModel>();
            return PartialView("~/Areas/User/Views/Alcohol/_Products.cshtml", responseResult);

        }

    }
}