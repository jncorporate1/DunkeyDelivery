using DunkeyDelivery.Areas.User.Models;
using DunkeyDelivery.BindingModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.User.Controllers
{
    [HandleError]
    public class FoodController : Controller
    {
        // GET: User/Food
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> SearchStoreCategories(SearchStoreCategoriesViewModel model)
        {

            //IEnumerable<CategoryViewModel> modelResponse;
            CategoryProductViewModel modelResponse = new CategoryProductViewModel();
            #region GetProduct

            var responseShop = await ApiCall<ShopViewModel>.CallApi("api/Shop/GetStoreById?Id=" + model.Store_id + "", null, false);
            var responseShopValue = responseShop.GetValue("Result").ToObject<ShopViewModel>();
            if (responseShopValue.ImageUrl == null)
            {
                responseShopValue.ImageUrl = DefaultImages.StoreDefaultImage();
            }
            modelResponse.shopViewModel = responseShopValue;
            ViewBag.StoreDetails = responseShopValue;


            #endregion

            #region GetCategories
            var response = await ApiCall<CategoryViewModel>.CallApi("api/Shop/GetStoreCategories?Store_id=" + model.Store_id + "", null, false);


            var responseValue = response.GetValue("Result").ToObject<IEnumerable<CategoryViewModel>>();
            #endregion

            #region GetStoreReviews
            var responseReviews = await ApiCall<ReviewViewModel>.CallApi("api/Shop/GetStoreReviews?Store_Id=" + model.Store_id + "", null, false);
            var responseReviewValue = responseReviews.GetValue("Result").ToObject<ReviewViewModel>();
            #endregion


            modelResponse.Categories = responseValue;
            modelResponse.ReviewForView = responseReviewValue;

            ViewBag.BannerImage = "press-top-banner.jpg";
            ViewBag.Title = " Store Details";
            ViewBag.BannerTitle = "Restaurant's Categories ";
            ViewBag.Path = "Home > Categories";

            modelResponse.SetSharedData(User);
            return View("~/Areas/User/Views/Food/FoodDetails.cshtml", modelResponse);


        }
        public ActionResult GetCategoryProducts(short Category_Id, SearchStoreCategoriesViewModel model)
        {
            CategoryProductViewModel modelResponse = new CategoryProductViewModel();
            #region GetProducts
            var responseProduct = AsyncHelpers.RunSync<JObject>(() => ApiCall<ProductViewModel>.CallApi("api/Products/ProductsByCategory?Category_Id=" + model.Category_Id + "", null, false));
            var responseValueProduct = responseProduct.GetValue("Result").ToObject<IEnumerable<ProductViewModel>>();

            #endregion
          //  var test = responseValueProduct.First().Store.BusinessName;
            modelResponse.Products = responseValueProduct;
            modelResponse.shopViewModel = new ShopViewModel { Id = model.Store_id, BusinessType = model.BusinessType, BusinessTypeTax = model.BusinessTypeTax};
            return PartialView("~/Areas/User/Views/Food/_FoodDetails.cshtml", modelResponse);
        }

        public ActionResult GetStoreList(Shop model)
        {
            return PartialView("~/Areas/User/Views/Food/_StoresList.cshtml", model);
        }

        public async Task<ActionResult> SideBarData(string CategoryType)
        {
            var SideBarData = AsyncHelpers.RunSync<JObject>(() => ApiCall<ShopViewModel>.CallApi("api/Shop/SideBarData?CategoryType="+CategoryType, null, false));
            var response = SideBarData.GetValue("Result").ToObject<SideBar>();
            return PartialView("~/Areas/User/Views/Store/_SideBar.cshtml", response);
        }

    }
}