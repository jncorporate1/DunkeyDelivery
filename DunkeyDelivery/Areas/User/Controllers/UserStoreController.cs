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
    public class UserStoreController : Controller
    {
        // GET: User/UserStore
        public ActionResult Index()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }



        public async Task<ActionResult> FilterStore(string FilterType, string CategoryType, int Items = 0, int Page = 0)
        {

            try
            {
                if (Items == 0 && Page == 0)
                {
                    if (FilterType == "Food")
                    {
                        Items = 6;
                        Page = 0;
                    }
                    else
                    {
                        Items = 5;
                        Page = 0;
                    }
                }
                var response = await ApiCall<Shop>.CallApi("api/Shop/GetFilteredStores?FilterType=" + FilterType + "&CategoryType=" + CategoryType + "&Items=" + Items + "&Page=" + Page+"&CurrentTime="+DateTime.Now, null, false);

                var responseShopValue = response.GetValue("Result").ToObject<Shop>();
                #region SettingDefaultStoreImage
                foreach (var shop in responseShopValue.Store)
                {
                    if (shop.ImageUrl == null)
                    {

                        shop.ImageUrl = DefaultImages.StoreDefaultImage();
                    }
                }
                #endregion

                //    return PartialView("~/Areas/User/Views/Food/_StoresList.cshtml", responseShopValue);
                if (CategoryType == "Food")
                {
                    return PartialView("~/Areas/User/Views/Food/_StoresList.cshtml", responseShopValue);

                }
                else
                {
                    return PartialView("~/Areas/User/Views/Store/_AllStoreList.cshtml", responseShopValue);

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<ActionResult> SearchStoreCategories(SearchStoreCategoriesViewModel model)
        {

            var Shop = await ApiCall<ShopViewModel>.CallApi("api/Shop/GetStoreById?Id=" + model.Store_id + "", null, false);
            
            if (model.SearchType == "Laundry")
            {
                #region Laundry Store 

                LaundryProductsInCategory viewmodel = new LaundryProductsInCategory();

                var cat = await ApiCall<List<LundryProductVM>>.CallApi("api/Categories/GetCategoryProducts?Store_id=" + model.Store_id + "", null, false);

                var laundryproducts = cat.GetValue("Result").ToObject<List<LundryProductVM>>();
                viewmodel.laundryproducts = laundryproducts;

                var responseShop = Shop.GetValue("Result").ToObject<ShopViewModel>();

                #region SettingDefaultStoreImage
                
                    if (responseShop.ImageUrl == null)
                    {

                    responseShop.ImageUrl = DefaultImages.StoreDefaultImage();
                    }
                
                #endregion


                viewmodel.StoreViewModel = responseShop;
                viewmodel.SetSharedData(User);
                return View("~/Areas/User/Views/Laundry/LaundryDetails.cshtml", viewmodel);
                #endregion
            }
            else if (model.SearchType == "Alcohol")
            {
                #region Alcohol Store 
                AlcoholViewModel viewModel =new AlcoholViewModel();
                var apiResponse = await ApiCall<List<CategoryViewModel>>.CallApi("api/Shop/GetStoreCategories?Store_id=" + model.Store_id + "", null, false);
                viewModel.categoryViewModel.CategoryViewModel = apiResponse.GetValue("Result").ToObject<List<CategoryViewModel>>();

                viewModel.storeViewModel = Shop.GetValue("Result").ToObject<ShopViewModel>();
                #region SettingDefaultStoreImage

                if (viewModel.storeViewModel.ImageUrl == null)
                {

                    viewModel.storeViewModel.ImageUrl = DefaultImages.StoreDefaultImage();
                }

                #endregion
                viewModel.SetSharedData(User);
                return View("~/Areas/User/Views/Alcohol/StoreCategories.cshtml", viewModel);
                #endregion
            }
            else if (model.SearchType == "Pharmacy")
            {


                PharmacyViewModel response = new PharmacyViewModel();
                response.storeViewModel = Shop.GetValue("Result").ToObject<ShopViewModel>();
                #region SettingDefaultStoreImage

                if (response.storeViewModel.ImageUrl == null)
                {

                    response.storeViewModel.ImageUrl = DefaultImages.StoreDefaultImage();
                }

                #endregion
                response.SetSharedData(User);
                return View("~/Areas/User/Views/Pharmacy/PharmacyDetails.cshtml", response);
            }
            else if (model.SearchType == "Retail")
            {
                #region Retailer Copy Of Alcohol
                AlcoholViewModel viewModel = new AlcoholViewModel();
                var apiResponse = await ApiCall<List<CategoryViewModel>>.CallApi("api/Shop/GetStoreCategories?Store_id=" + model.Store_id + "", null, false);
                viewModel.categoryViewModel.CategoryViewModel = apiResponse.GetValue("Result").ToObject<List<CategoryViewModel>>();

                viewModel.storeViewModel = Shop.GetValue("Result").ToObject<ShopViewModel>();
                #region SettingDefaultStoreImage

                if (viewModel.storeViewModel.ImageUrl == null)
                {

                    viewModel.storeViewModel.ImageUrl = DefaultImages.StoreDefaultImage();
                }

                #endregion
                viewModel.SetSharedData(User);
                return View("~/Areas/User/Views/Retail/RetailDetails.cshtml",viewModel);
                #endregion
            }
            else
            {

                CategoryProductViewModel modelResponse = new CategoryProductViewModel();
                #region GetProduct


                modelResponse.shopViewModel = Shop.GetValue("Result").ToObject<ShopViewModel>();
                #endregion

                #region GetCategories
                var response = await ApiCall<CategoryViewModel>.CallApi("api/Shop/GetStoreCategories?Store_id=" + model.Store_id + "", null, false);


                modelResponse.Categories = response.GetValue("Result").ToObject<IEnumerable<CategoryViewModel>>();
                #endregion



                #region SettingDefaultStoreImage

                if (modelResponse.shopViewModel.ImageUrl == null)
                {

                    modelResponse.shopViewModel.ImageUrl = DefaultImages.StoreDefaultImage();
                }

                #endregion


                ViewBag.BannerImage = "press-top-banner.jpg";
                ViewBag.Title = "Store Details";
                ViewBag.BannerTitle = "Restaurant's Categories ";
                ViewBag.Path = "Home > Categories";
                modelResponse.SetSharedData(User);
                return View("~/Areas/User/Views/Product/ProductDetails.cshtml", modelResponse);
            }



        }
        public ActionResult GetCategoryProducts(short Category_Id, SearchStoreCategoriesViewModel model)
        {
            {
                CategoryProductViewModel modelResponse = new CategoryProductViewModel();
                #region GetProducts
                var responseProduct = AsyncHelpers.RunSync<JObject>(() => ApiCall<ProductViewModel>.CallApi("api/Products/ProductsByCategory?Category_Id=" + model.Category_Id + "", null, false));
                var responseValueProduct = responseProduct.GetValue("Result").ToObject<IEnumerable<ProductViewModel>>();

                #endregion
                modelResponse.Products = responseValueProduct;

                return PartialView("~/Areas/User/Views/Food/_FoodDetails.cshtml", modelResponse);
                // return PartialView("~/Areas/User/Views/Product/_ProductItems.cshtml", modelResponse);

                //return View("~/Areas/User/Views/Grocery/GroceryDetails.cshtml", modelResponse);

            }
        }
        public JsonResult JsonGetCategoryProducts(short Category_Id, SearchStoreCategoriesViewModel model)
        {
            {
                CategoryProductViewModel modelResponse = new CategoryProductViewModel();
                #region GetProducts
                var responseProduct = AsyncHelpers.RunSync<JObject>(() => ApiCall<ProductViewModel>.CallApi("api/Products/ProductsByCategory?Category_Id=" + model.Category_Id + "", null, false));
                var responseValueProduct = responseProduct.GetValue("Result").ToObject<IEnumerable<ProductViewModel>>();

                #endregion
                modelResponse.Products = responseValueProduct;
                return Json(modelResponse.Products, JsonRequestBehavior.AllowGet);
                //return PartialView("~/Areas/User/Views/Grocery/_ProductsDetails.cshtml", modelResponse);

                //return View("~/Areas/User/Views/Grocery/GroceryDetails.cshtml", modelResponse);

            }
        }
        public ActionResult GetAllStoreList(Shop model)
        {
            return PartialView("~/Areas/User/Views/Store/_AllStoreList.cshtml", model);
        }
        public ActionResult StoreInfo(CategoryProductViewModel Model)
        {
     
            return PartialView("~/Areas/User/Views/Store/_StoreInfo.cshtml", Model);
        }







    }
}