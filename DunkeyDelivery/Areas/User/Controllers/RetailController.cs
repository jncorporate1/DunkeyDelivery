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
    public class RetailController : Controller
    {
        // GET: User/Retail
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RetailDetails()
        {
            return View("RetailDetails");
        }
        public ActionResult DetailProducts()
        {
            return View("DetailProducts");
        }
        public ActionResult StoreCategories(AlcoholViewModel model)
        {

            return PartialView("_Categories", model.categoryViewModel);
        }
        public async Task<ActionResult> subcategories(int Category_id, string Name, int Store_id)
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
    }
}