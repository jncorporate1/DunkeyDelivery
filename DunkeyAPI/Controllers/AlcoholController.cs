using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using DunkeyAPI.Models;
using DunkeyAPI.ViewModels;
using AutoMapper;
using DunkeyAPI.Utility;
using DunkeyAPI.ExtensionMethods;
using Z.EntityFramework.Plus;
using DunkeyDelivery;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Alcohol")]
    public class AlcoholController : ApiController
    {
        [HttpGet]
        [Route("AlcoholHomeScreen")]
        public async Task<IHttpActionResult> AlcoholHomeScreen(double latitude, double longitude, int? Page = 0, int? Items = 10, string Store_Ids = "")
        {
            try
            {
                AlcoholViewModel model = new AlcoholViewModel();
                AlcoholViewModel ReturnModel = new AlcoholViewModel();
                AlcoholViewModel ModifiedModel = new AlcoholViewModel();
                AlcoholViewModel copiedModel = new AlcoholViewModel();
                using (DunkeyContext ctx = new DunkeyContext())
                {




                    var Stores = new List<Store>();
                    string[] StoreIdsString;
                    int[] StoreIdsInt = new int[0];
                    var point = DunkeyDelivery.Utility.CreatePoint(latitude, longitude);
                    var CategoryIdInQuery = 0;
                    if (!string.IsNullOrEmpty(Store_Ids))
                    {
                        StoreIdsString = Store_Ids.Split(',');
                        StoreIdsInt = Array.ConvertAll(StoreIdsString, s => int.Parse(s));
                    }

                    if (StoreIdsInt.Length != 0)
                    {
                        Stores = ctx.Stores.Include(x => x.Categories).Include(x => x.Products).Include(x => x.StoreTags).Where(x => StoreIdsInt.Contains(x.Id) && x.BusinessType == Utility.Global.Constants.Alcohol && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                    }
                    else
                    {
                        Stores = ctx.Stores.IncludeFilter(x => x.Categories.Where(y => y.IsDeleted == false)).Include(x => x.Products).Include(x => x.StoreTags).Where(x => x.Location.Distance(point) < DunkeyDelivery.Global.NearbyStoreRadius && x.BusinessType == Utility.Global.Constants.Alcohol && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                    }


                    if (Stores.Count > 0)
                    {

                        foreach (var store in Stores)
                        {

                            if (store.Categories.Any(x => (x.Name.Contains("Wine") || x.Name.Contains("Liquor") || x.Name.Contains("Beer")) && x.ParentCategoryId == 0 && x.IsDeleted == false))
                            {
                                foreach (var cat in store.Categories)
                                {
                                    store.Categories = store.Categories.Where(x => x.IsDeleted == false && x.Store_Id == store.Id && x.ParentCategoryId == 0).ToList();
                                    var query = @"WITH    q AS 
                                                (
                                                SELECT  c.Id
                                                FROM    Categories c
                                                WHERE   c.Id = " + cat.Id + " UNION ALL  SELECT  ic.Id FROM    Categories ic JOIN    q ON      ic.ParentCategoryId =q.Id ) SELECT  Id As Ids FROM    q";
                                    var CategoryAndSubCategoryIds = ctx.Database.SqlQuery<int>(query).ToList();
                                    var str = String.Join(",", CategoryAndSubCategoryIds);
                                    var GetAllProducts = @"select * from Products Where Products.Category_Id IN (" + str + ") AND IsDeleted=0";
                                    var Products = ctx.Database.SqlQuery<Product>(GetAllProducts).ToList();

                                    foreach (var prod in Products)
                                    {
                                        prod.BusinessName = store.BusinessName;
                                        prod.BusinessType = store.BusinessType;
                                        prod.MinDeliveryCharges = store.MinDeliveryCharges;
                                        prod.MinDeliveryTime = store.MinDeliveryTime;
                                        prod.MinOrderPrice = store.MinOrderPrice;
                                        cat.Products = Products; /*cat.Products.Where(x => x.IsDeleted == false && x.Store_Id == cat.Store_Id).ToList();*/
                                        prod.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prod.Id && x.IsDeleted == false).ToList();

                                    }
                                }
                                store.Distance = store.Location.Distance(point).Value;

                                store.Distance = DunkeyDelivery.Utility.ConvertMeterToMile(store.Distance);
                                var tempDistance = store.Distance.ToString("0.00");
                                store.Distance = Convert.ToDouble(tempDistance);
                                store.CalculateAverageRating();
                                model.Stores.Add(store);
                            }

                        }
                        //Todo
                        var beerStore = model.Stores.FirstOrDefault(x => x.Categories.Any(y => y.Name == "Beer" && x.IsDeleted == false));
                        if (beerStore != null)
                        {
                            beerStore.CategoryType = (int)DunkeyDelivery.Stores.AlcoholCategoryTypes.Beer;
                        }
                        model.Stores = model.Stores.OrderBy(x => x.CategoryType).ToList();
                        //model.Stores = model.Stores.OrderBy(x => x.Categories).ToList
                    }





                    var UniqueProductSizesForFilter = "select DISTINCT ProductSizes.Unit,ProductSizes.Size,ProductSizes.TypeID,ProductSizes.NetWeight From ProductSizes Where ProductSizes.IsDeleted=0";
                    model.FilterProductSizes = ctx.Database.SqlQuery<AlcoholSizeFilters>(UniqueProductSizesForFilter).ToList();

                    var BeerStoreCategories = new List<Category>();
                    List<Store> stores = new List<Store>();
                    var beerAvailable = false;
                    int indx = 0;
                    int currentIndex = 0;
                    foreach (var store in model.Stores)
                    {
                        if (stores.Count == 0)
                        {
                            var WineStoreCategories = store.Categories.Where(x => x.Name.Contains("Wine") || x.Name.Contains("Liquor") && x.IsDeleted == false).ToList();
                            BeerStoreCategories = store.Categories.Where(x => x.Name.Contains("Beer") && x.IsDeleted == false).ToList();
                            if (WineStoreCategories.Count > 0)
                            {
                                store.Categories = WineStoreCategories;
                                stores.Add(store);
                            }
                            if (BeerStoreCategories.Count > 0 && !beerAvailable)
                            {
                                indx = currentIndex;
                                beerAvailable = true;
                            }

                        }
                        else
                        {

                            BeerStoreCategories = store.Categories.Where(x => x.Name.Contains("Beer") && x.IsDeleted == false).ToList();
                            if (BeerStoreCategories.Count > 0)
                            {
                                store.Categories = BeerStoreCategories;
                                stores.Add(store);
                            }

                        }
                        currentIndex++;
                    }

                    if (beerAvailable && stores.Count <= 1)
                    {
                        Store store = model.Stores[indx].Clone() as Store;
                        if (store != null)
                        {
                            store.Categories = BeerStoreCategories;
                            stores.Add(store);
                        }
                    }

                    ModifiedModel.Stores.Add(model.Stores.FirstOrDefault());

                    model.Stores = stores;

                    return Ok(new CustomResponse<AlcoholViewModel> { Message = Utility.Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("AlcoholStoreCategoryDetails")]
        public async Task<IHttpActionResult> AlcoholStoreCategoryDetails(int Store_Id, int? Category_Id = 0, int? Category_ParentId = 0, string CategoryName = "", int? Page = 0, int? Items = 10, int? Type_Id = 0)
        {

            // WARNING  !!!!!! Dont try to understand this service . your head will burn up . implemented this during training 
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var StoreDetails = ctx.Stores.FirstOrDefault(x => x.Id == Store_Id);
                    AlcoholStoreCategories model = new AlcoholStoreCategories();
                    AlcoholStoreParentCategories responseModel = new AlcoholStoreParentCategories();
                    //AlcoholChildCategories childModel = new AlcoholChildCategories();
                    var ParentIds = new List<int>();
                    var Category_Name = string.Empty;

                    var WineParentId = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Name.Contains("Wine") && x.Store_Id == Store_Id && x.IsDeleted == false).FirstOrDefault();
                    var LiquorParentId = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Store_Id == Store_Id && x.Name.Contains("Liquor") && x.IsDeleted == false).FirstOrDefault();
                    var BeerParentId = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Store_Id == Store_Id && x.Name.Contains("Beer") && x.IsDeleted == false).FirstOrDefault();

                    if (Category_Id.HasValue)
                    {
                        Category_Name = ctx.Categories.FirstOrDefault(x => x.Id == Category_Id && x.IsDeleted == false).Name;
                    }
                    if (Category_ParentId.Value == 0)
                    {
                        if (WineParentId != null)
                        {
                            responseModel.Categories.Wine = ctx.Categories.IncludeFilter(x => x.Products.Where(y=>y.IsDeleted==false)).Where(x => x.ParentCategoryId.Value == WineParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        }
                        if (LiquorParentId != null)
                        {
                            responseModel.Categories.Liquor = ctx.Categories.IncludeFilter(x => x.Products.Where(y => y.IsDeleted == false)).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == LiquorParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        }
                        if (BeerParentId != null)
                        {
                            responseModel.Categories.Beer = ctx.Categories.IncludeFilter(x => x.Products.Where(y => y.IsDeleted == false)).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == BeerParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        }
                        // For WIne section no sub category result
                        if (responseModel.Categories.Wine.Count == 0 && Category_ParentId == 0)
                        {
                            if (Category_Name == "Wine")
                            {
                                responseModel.WineLastProducts = ctx.Products.Where(x => x.Category_Id == Category_Id && x.Store_Id == Store_Id && x.IsDeleted == false).ToList();

                                foreach (var prodDetail in responseModel.WineLastProducts)
                                {
                                    prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                    prodDetail.BusinessName = StoreDetails.BusinessName;
                                    prodDetail.BusinessType = StoreDetails.BusinessType;
                                    prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                    prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                    prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();
                                    prodDetail.Store = null;
                                }

                                responseModel.IsLast = true;
                            }
                        }


                        // end here 
                        foreach (var prod in responseModel.Categories.Wine)
                        {
                            if (prod.Products.Count > 0)
                            {
                                foreach (var prodDetail in prod.Products)
                                {
                                    prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                    prodDetail.BusinessName = StoreDetails.BusinessName;
                                    prodDetail.BusinessType = StoreDetails.BusinessType;
                                    prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                    prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                    prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();
                                }
                            }
                            else
                            {
                                prod.Products = ctx.Products.Where(x => x.Store_Id == Store_Id && x.Category_Id == Category_ParentId && x.IsDeleted == false).ToList();
                            }
                        }

                        //start from here
                        if (responseModel.Categories.Liquor.Count == 0 && Category_ParentId == 0)
                        {
                            if (Category_Name == "Liquor")
                            {
                                responseModel.LiquorLastProducts = ctx.Products.Where(x => x.Store_Id == Store_Id && x.Category_Id == Category_Id && x.IsDeleted == false).ToList();
                                foreach (var prodDetail in responseModel.LiquorLastProducts)
                                {
                                    prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                    prodDetail.BusinessName = StoreDetails.BusinessName;
                                    prodDetail.BusinessType = StoreDetails.BusinessType;
                                    prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                    prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                    prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();
                                    prodDetail.Store = null;
                                }
                                responseModel.IsLast = true;
                            }
                        }
                        // end here
                        foreach (var prod in responseModel.Categories.Liquor)
                        {
                            if (prod.Products.Count > 0)
                            {
                                foreach (var prodDetail in prod.Products)
                                {
                                    prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                    prodDetail.BusinessName = StoreDetails.BusinessName;
                                    prodDetail.BusinessType = StoreDetails.BusinessType;
                                    prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                    prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                    prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();

                                }
                            }
                            else
                            {
                                prod.Products = ctx.Products.Where(x => x.Store_Id == Store_Id && x.Category_Id == Category_ParentId && x.IsDeleted == false).ToList();
                            }
                        }
                        if (responseModel.Categories.Beer.Count > 0 && Category_ParentId != 0)
                        {
                            foreach (var prod in responseModel.Categories.Beer)
                            {
                                //if (prod.Products.Count > 0)
                                //{


                                foreach (var prodDetail in prod.Products)
                                {
                                    prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                    prodDetail.BusinessName = StoreDetails.BusinessName;
                                    prodDetail.BusinessType = StoreDetails.BusinessType;
                                    prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                    prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                    prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();

                                }
                                //}
                                //else
                                //{
                                //    prod.Products = ctx.Products.Where(x => x.Store_Id == Store_Id && x.Category_Id == Category_ParentId && x.IsDeleted == false).ToList();
                                //}
                            }
                        }
                        else if (responseModel.Categories.Beer.Count == 0 && Category_ParentId == 0)
                        {
                            if (Category_Name == "Beer")
                            {
                                responseModel.BeerLastProducts = ctx.Products.Where(x => x.Store_Id == Store_Id && x.Category_Id == Category_Id && x.IsDeleted == false).ToList();
                                foreach (var prodDetail in responseModel.BeerLastProducts)
                                {
                                    prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                    prodDetail.BusinessName = StoreDetails.BusinessName;
                                    prodDetail.BusinessType = StoreDetails.BusinessType;
                                    prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                    prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                    prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();
                                    prodDetail.Store = null;
                                }
                                responseModel.IsLast = true;
                            }
                        }
                        else
                        {
                            responseModel.IsLast = true;
                            responseModel.BeerLastProducts = ctx.Products.Where(x => x.Category_Id == Category_ParentId || x.Category_Id == Category_Id && x.IsDeleted == false).ToList();
                        }
                        return Ok(new CustomResponse<AlcoholStoreParentCategories> { Message = Utility.Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });

                    }
                    else if (!string.IsNullOrEmpty(CategoryName))
                    {
                        if (CategoryName.Contains("Wine"))
                        {
                            responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                            if (responseModel.Categories.Wine.Count == 0 && Category_Id != 0)
                            {

                                var query = @"WITH    q AS 
                                                (
                                                SELECT  c.Id
                                                FROM    Categories c
                                                WHERE   c.ParentCategoryId = 0 AND c.Name='Wine' AND Store_Id='"+Store_Id+"' UNION ALL  SELECT  ic.Id FROM    Categories ic JOIN    q ON      ic.ParentCategoryId =q.Id ) SELECT  Id As Ids FROM    q";
                                var CategoryAndSubCategoryIds = ctx.Database.SqlQuery<int>(query).ToList();
                                var str = String.Join(",", CategoryAndSubCategoryIds);
                                var GetAllProducts = @"select * from Products Where Products.Category_Id IN (" + str + ") AND IsDeleted=0";
                                var Products = ctx.Database.SqlQuery<Product>(GetAllProducts).ToList();
                                responseModel.Products = Products;
                                //responseModel.Products = ctx.Products.Where(x => x.Store.Id == Store_Id && x.Category_Id == Category_Id.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.IsLast = true;
                            }

                            if (LiquorParentId != null)
                            {
                                responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == LiquorParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                            }
                            if (BeerParentId != null)
                            {
                                responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == BeerParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                            }
                        }
                        else if (CategoryName.Contains("Liquor"))
                        {
                            responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == WineParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                            responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                            if (responseModel.Categories.Liquor.Count == 0 && Category_Id != 0)
                            {
                                var query = @"WITH    q AS 
                                                (
                                                SELECT  c.Id
                                                FROM    Categories c
                                                WHERE   c.ParentCategoryId = 0 AND c.Name='Liquor' AND Store_Id='" + Store_Id + "' UNION ALL  SELECT  ic.Id FROM    Categories ic JOIN    q ON      ic.ParentCategoryId =q.Id ) SELECT  Id As Ids FROM    q";
                                var CategoryAndSubCategoryIds = ctx.Database.SqlQuery<int>(query).ToList();
                                var str = String.Join(",", CategoryAndSubCategoryIds);
                                var GetAllProducts = @"select * from Products Where Products.Category_Id IN (" + str + ") AND IsDeleted=0";
                                var Products = ctx.Database.SqlQuery<Product>(GetAllProducts).ToList();
                                responseModel.Products = Products;
                                //responseModel.Products = ctx.Products.Where(x => x.Store.Id == Store_Id && x.Category_Id == Category_Id.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.IsLast = true;
                            }
                            if (BeerParentId != null)
                            {
                                responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == BeerParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                            }

                        }
                        else
                        {
                            if (WineParentId != null)
                                responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == WineParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                            if (LiquorParentId != null)
                            {
                                responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == LiquorParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                            }
                            responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                            if (responseModel.Categories.Beer.Count == 0 && Category_Id != 0)
                            {
                                var query = @"WITH    q AS 
                                                (
                                                SELECT  c.Id
                                                FROM    Categories c
                                                WHERE   c.ParentCategoryId = 0 AND c.Name='Beer' AND Store_Id='" + Store_Id + "' UNION ALL  SELECT  ic.Id FROM    Categories ic JOIN    q ON      ic.ParentCategoryId =q.Id ) SELECT  Id As Ids FROM    q";
                                var CategoryAndSubCategoryIds = ctx.Database.SqlQuery<int>(query).ToList();
                                var str = String.Join(",", CategoryAndSubCategoryIds);
                                var GetAllProducts = @"select * from Products Where Products.Category_Id IN (" + str + ") AND IsDeleted=0";
                                var Products = ctx.Database.SqlQuery<Product>(GetAllProducts).ToList();
                                responseModel.Products = Products;
                                //responseModel.Products = ctx.Products.Where(x => x.Store.Id == Store_Id && x.Category_Id == Category_Id.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.IsLast = true;
                            }

                        }
                        foreach (var prod in responseModel.Categories.Wine)
                        {
                            if (prod.Products.Count > 0)
                            {
                                foreach (var prodDetail in prod.Products)
                                {
                                    prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                    prodDetail.BusinessName = StoreDetails.BusinessName;
                                    prodDetail.BusinessType = StoreDetails.BusinessType;
                                    prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                    prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                    prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();


                                }
                            }
                            else
                            {
                                //prod.Products = ctx.Products.Where(x => x.Store_Id == Store_Id && x.Category_Id == Category_ParentId && x.IsDeleted == false).ToList();
                            }
                        }
                        foreach (var prod in responseModel.Categories.Liquor)
                        {
                            if (prod.Products.Count > 0)
                            {

                                foreach (var prodDetail in prod.Products)
                                {
                                    prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                    prodDetail.BusinessName = StoreDetails.BusinessName;
                                    prodDetail.BusinessType = StoreDetails.BusinessType;
                                    prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                    prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                    prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();

                                }
                            }
                            else
                            {
                                //prod.Products = ctx.Products.Where(x => x.Store_Id == Store_Id && x.Category_Id == Category_ParentId && x.IsDeleted == false).ToList();

                            }
                        }
                        foreach (var prod in responseModel.Categories.Beer)
                        {
                            if (prod.Products.Count > 0)
                            {

                                foreach (var prodDetail in prod.Products)
                                {
                                    prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                    prodDetail.BusinessName = StoreDetails.BusinessName;
                                    prodDetail.BusinessType = StoreDetails.BusinessType;
                                    prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                    prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                    prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();
                                }
                            }
                            else
                            {
                                //prod.Products = ctx.Products.Where(x => x.Store_Id == Store_Id && x.Category_Id == Category_ParentId && x.IsDeleted == false).ToList();

                            }
                        }

                        if (responseModel.Products != null)
                        {
                            foreach (var prodDetail in responseModel.Products)
                            {
                                prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                prodDetail.BusinessName = StoreDetails.BusinessName;
                                prodDetail.BusinessType = StoreDetails.BusinessType;
                                prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;
                                prodDetail.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prodDetail.Id).ToList();

                            }
                        }

                        return Ok(new CustomResponse<AlcoholStoreParentCategories> { Message = Utility.Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });


                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CategoryName))
                        {
                            if (CategoryName.Contains("Wine"))
                            {
                                responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                                if (responseModel.Categories.Wine.Count == 0 && Category_Id != 0)
                                {
                                    responseModel.Products = ctx.Products.Where(x => x.Store.Id == Store_Id && x.Category_Id == Category_Id.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                    responseModel.IsLast = true;
                                }
                                if (LiquorParentId != null)
                                {
                                    responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == LiquorParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                }
                                if (BeerParentId != null)
                                {
                                    responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == BeerParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                }
                            }
                            else if (CategoryName.Contains("Liquor"))
                            {
                                responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == WineParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                                if (responseModel.Categories.Liquor.Count == 0 && Category_Id != 0)
                                {
                                    responseModel.Products = ctx.Products.Where(x => x.Store.Id == Store_Id && x.Category_Id == Category_Id.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                    responseModel.IsLast = true;
                                }

                                responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == BeerParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                            }
                            else
                            {
                                responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == WineParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == LiquorParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                                if (responseModel.Categories.Beer.Count == 0 && Category_Id != 0)
                                {
                                    responseModel.Products = ctx.Products.Where(x => x.Store.Id == Store_Id && x.Category_Id == Category_Id.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                    responseModel.IsLast = true;
                                }

                            }

                        }

                        foreach (var prod in responseModel.Categories.Wine)
                        {
                            foreach (var prodDetail in prod.Products)
                            {
                                prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                prodDetail.BusinessName = StoreDetails.BusinessName;
                                prodDetail.BusinessType = StoreDetails.BusinessType;
                                prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;


                            }
                        }
                        foreach (var prod in responseModel.Categories.Liquor)
                        {
                            foreach (var prodDetail in prod.Products)
                            {
                                prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                prodDetail.BusinessName = StoreDetails.BusinessName;
                                prodDetail.BusinessType = StoreDetails.BusinessType;
                                prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;


                            }
                        }
                        foreach (var prod in responseModel.Categories.Beer)
                        {
                            foreach (var prodDetail in prod.Products)
                            {
                                prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                prodDetail.BusinessName = StoreDetails.BusinessName;
                                prodDetail.BusinessType = StoreDetails.BusinessType;
                                prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;


                            }
                        }

                        if (responseModel.Products != null)
                        {
                            foreach (var prodDetail in responseModel.Products)
                            {
                                prodDetail.MinDeliveryCharges = StoreDetails.MinDeliveryCharges;
                                prodDetail.BusinessName = StoreDetails.BusinessName;
                                prodDetail.BusinessType = StoreDetails.BusinessType;
                                prodDetail.MinDeliveryTime = StoreDetails.MinDeliveryTime;
                                prodDetail.MinOrderPrice = StoreDetails.MinOrderPrice;


                            }
                        }

                        return Ok(new CustomResponse<AlcoholStoreParentCategories> { Message = Utility.Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });

                    }


                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("AlcoholFilterTypeStoreCategoryDetails")]
        public async Task<IHttpActionResult> AlcoholFilterTypeStoreCategoryDetails(int? SortBy = 0, string Country = "", string Price = "", string Size = "", int Type_Id = 0, string ParentName = "", string ProductNetWeight = "")
        {
            try
            {
                //FilterStoreViewModel model = new FilterStoreViewModel();
                //AlcoholViewModel returnModel = new AlcoholViewModel();
                AlcoholStoreParentCategories returnModel = new AlcoholStoreParentCategories();

                string[] CountryList;
                int[] CountryListInt;

                string[] PriceList;
                int[] PriceListInt;


                string[] SizeList;
                int[] SizeListInt;

                string ExtendedQuery = "";
                string ExtendedWhere = "";

                List<string> NetWeightList;


                var query = @"SELECT  Products.*
                FROM Products 
                join Stores on Products.Store_Id = Stores.Id
                left join ProductSizes on Products.Id=ProductSizes.Product_Id
			    LEFT JOIN storeratings 
                ON stores.id = storeratings.store_id WHERE stores.businesstype = 'Alcohol' AND   ";

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (!string.IsNullOrEmpty(Country))
                    {
                        CountryList = Country.Split(',');

                        for (var i = 1; i <= CountryList.Count(); i++)
                        {
                            if (i == CountryList.Count())
                            {
                                query += "Stores.Address LIKE '%" + CountryList[i - 1] + "%' AND ";

                            }
                            else
                            {
                                query += "Stores.Address LIKE '%" + CountryList[i - 1] + "%' OR ";
                            }
                        }
                    }




                    if (!string.IsNullOrEmpty(Price))
                    {
                        PriceList = Price.Split(',');
                        PriceListInt = Array.ConvertAll(PriceList, s => int.Parse(s));

                        if (PriceListInt.Count() == 1)
                        {
                            query += "  Products.Price > " + PriceListInt[0] + " AND ";
                        }
                        else if (PriceListInt.Count() == 2)
                        {
                            query += " (Products.Price >= " + PriceListInt.Min() + " AND Products.Price <= " + PriceListInt.Max() + "  ) AND ";
                        }

                    }


                    if (!string.IsNullOrEmpty(ProductNetWeight))
                    {

                        NetWeightList = ProductNetWeight.Split('#').ToList();
                        query += " ProductSizes.NetWeight in (";
                        for (var i = 0; i < NetWeightList.Count; i++)
                        {
                            if (i == NetWeightList.Count - 1)
                                query += "'" + NetWeightList[i] + "'";
                            else
                                query += "'" + NetWeightList[i] + "',";

                        }
                        query += ") AND ";



                    }

                    query += " stores.isdeleted = 'false'  ";
                    if (!string.IsNullOrEmpty(Size))
                    {
                        //SizeList = Size.Split(',');
                        //SizeListInt = Array.ConvertAll(SizeList, s => int.Parse(s));
                    }


                    if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.BestSelling)
                    {
                        query += " ORDER BY Stores.Id  DESC ";
                    }
                    else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.A2Z)
                    {
                        query += "  Order by Stores.BusinessName ASC ";
                    }
                    else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.Low2High)
                    {
                        query += "  Order by Products.Price ASC ";
                    }
                    else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.High2Low)
                    {
                        query += "  Order by Products.Price DESC ";
                    }
                    else
                    {
                        query += " ORDER BY Stores.Store_Id DESC";
                    }

                    var products = ctx.Database.SqlQuery<Product>(query).ToList();
                    if (products.Count == 0)
                    {

                    }
                    else
                    {
                        foreach (var item in products)
                        {
                            var store = ctx.Stores.FirstOrDefault(x => x.Id == item.Store_Id);
                            item.BusinessName = store.BusinessName;
                            item.BusinessType = store.BusinessType;
                            item.MinDeliveryCharges = store.MinDeliveryCharges;
                            item.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == item.Id).ToList();

                        }
                    }

                    var SubCategories = ctx.Categories.Where(x => x.ParentCategoryId == Type_Id && x.IsDeleted == false).ToList();

                    if (SubCategories.Count == 0)
                    {
                        returnModel.Products = products;
                        returnModel.IsLast = true;
                    }
                    else
                    {
                        foreach (var cat in SubCategories)
                        {
                            foreach (var item in products)
                            {
                                if (cat.Id == item.Category_Id)
                                {
                                    cat.Products.Add(item);
                                }
                            }

                        }

                        if (ParentName.Contains("Wine"))
                        {
                            returnModel.Categories.Wine = SubCategories;
                        }
                        else if (ParentName.Contains("Liquor"))
                        {
                            returnModel.Categories.Liquor = SubCategories;
                        }
                        else
                        {
                            returnModel.Categories.Beer = SubCategories;
                        }
                    }

                }
                CustomResponse<AlcoholStoreParentCategories> reponse = new CustomResponse<AlcoholStoreParentCategories>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = returnModel
                };

                return Ok(reponse);

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("ChangeStore")]
        public async Task<IHttpActionResult> ChangeStore(int? Type, double latitude, double longitude, int? Page = 0, int? Items = 10)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    List<Category> Categories = new List<Category>();
                    AlcoholViewModel model = new AlcoholViewModel();
                    var point = DunkeyDelivery.Utility.CreatePoint(latitude, longitude);
                    if (Type == 0) // for wine 
                    {
                        model.Stores = ctx.Stores.Where(x => x.BusinessType == Utility.Global.Constants.Alcohol && x.Categories.Any(y => y.Name.Contains("Wine")) && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        model.TotalRecords = ctx.Stores.Count(x => x.BusinessType == Utility.Global.Constants.Alcohol && x.Categories.Any(y => y.Name.Contains("Wine")) && x.IsDeleted == false);
                    }
                    else if (Type == 1) // for liquor
                    {
                        model.Stores = ctx.Stores.Where(x => x.BusinessType == Utility.Global.Constants.Alcohol && x.Categories.Any(y => y.Name.Contains("Liquor")) && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        model.TotalRecords = ctx.Stores.Count(x => x.BusinessType == Utility.Global.Constants.Alcohol && x.Categories.Any(y => y.Name.Contains("Liquor")) && x.IsDeleted == false);
                    }
                    else if (Type == 2) // for Vine & Liquor
                    {
                        model.Stores = ctx.Stores.Where(x => x.BusinessType == Utility.Global.Constants.Alcohol && x.Categories.Any(y => y.Name.Contains("Wine") || y.Name.Contains("Liquor")) && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        model.TotalRecords = ctx.Stores.Count(x => x.BusinessType == Utility.Global.Constants.Alcohol && x.Categories.Any(y => y.Name.Contains("Wine") || y.Name.Contains("Liquor")) && x.IsDeleted == false);
                    }
                    else // for beer
                    {
                        model.Stores = ctx.Stores.Where(x => x.BusinessType == Utility.Global.Constants.Alcohol && x.Categories.Any(y => y.Name.Contains("Beer")) && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        model.TotalRecords = ctx.Stores.Count(x => x.BusinessType == Utility.Global.Constants.Alcohol && x.Categories.Any(y => y.Name.Contains("Beer")) && x.IsDeleted == false);
                    }

                    foreach (var store in model.Stores)
                    {
                        store.Distance = store.Location.Distance(point).Value;

                        store.Distance = DunkeyDelivery.Utility.ConvertMeterToMile(store.Distance);
                        var tempDistance = store.Distance.ToString("0.00");
                        store.Distance = Convert.ToDouble(tempDistance);

                        store.CalculateAverageRating();
                    }


                    return Ok(new CustomResponse<AlcoholViewModel> { Message = Utility.Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("AlcoholFilterStore")]
        public IHttpActionResult AlcoholFilterStore(double latitude, double longitude, int? SortBy = 0, string Country = "", string Price = "", string ProductNetWeight = "")
        {
            try
            {
                FilterStoreViewModel model = new FilterStoreViewModel();
                AlcoholViewModel returnModel = new AlcoholViewModel();

                List<int> PriceList;
                List<string> NetWeightList;


                //Price rnage
                var query = @"SELECT  Products.*
                FROM Products 
                join Stores on Products.Store_Id = Stores.Id
                left join ProductSizes on Products.Id=ProductSizes.Product_Id
			    LEFT JOIN storeratings 
                ON stores.id = storeratings.store_id WHERE stores.businesstype = 'Alcohol' AND   ";

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (!string.IsNullOrEmpty(Price))
                    {
                        PriceList = Price.Split(',').Select(int.Parse).ToList();

                        if (PriceList.Count == 1)
                        {
                            query += "  Products.Price >= " + PriceList.First() + " AND ";
                        }
                        else if (PriceList.Count == 2)
                        {
                            query += " (Products.Price >= " + PriceList.Min() + " AND Products.Price <= " + PriceList.Max() + "  ) AND ";
                        }


                    }
                    if (!string.IsNullOrEmpty(ProductNetWeight))
                    {

                        NetWeightList = ProductNetWeight.Split('#').ToList();
                        query += " ProductSizes.NetWeight in (";
                        for (var i = 0; i < NetWeightList.Count; i++)
                        {
                            if (i == NetWeightList.Count - 1)
                                query += "'" + NetWeightList[i] + "'";
                            else
                                query += "'" + NetWeightList[i] + "',";

                        }
                        query += ") AND ";



                    }

                    query += " stores.isdeleted = '0' AND Products.isdeleted='0'  ";

                    List<string> countryList = new List<string>();
                    if (Country != null)
                    {

                        countryList = Country.Split(',').ToList();
                        //Country
                        if (countryList != null && countryList.Count > 0)
                        {
                            query += " and stores.Address like '%" + String.Join(",", countryList) + "%'";
                        }

                    }
                    //Sort By
                    #region SortBy

                    if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.BestSelling) ;
                    //query += " ORDER BY Stores.Id  DESC ";
                    else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.A2Z)
                        query += "  Order by Products.Name ASC ";
                    else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.Z2A)
                        query += "  Order by Products.Name DESC ";
                    else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.Low2High)
                        query += "  Order by Products.Price ASC ";
                    else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.High2Low)
                        query += "  Order by Products.Price DESC ";
                    else
                        query += " ORDER BY Stores.Store_Id DESC";
                    #endregion

                    var product = ctx.Database.SqlQuery<ProductAlcoholFilterModel>(query).ToList();

                    //Making return view model
                    List<Store> stores = new List<Store>();
                    List<Category> Categories = new List<Category>();




                    // umer code from here onwards
                    var uniqueStores = product.Select(x => x.Store_Id).Distinct();
                    //stores = ctx.Stores.Where(x => uniqueStores.Contains(x.Id)).ToList();

                    // getting categories of those products
                    var uniqueCategories = product.Select(x => x.Category_Id).Distinct().ToList();

                    //Add parent if not added

                    if (uniqueCategories.Count > 0)
                    {
                        var ff = @"WITH q AS 
                                                (
                                                SELECT  c.Id
                                                FROM    Categories c
                                                where c.Id in (" + String.Join(",", uniqueCategories) + @")
												UNION ALL
												SELECT  ic.ParentCategoryId FROM    Categories ic JOIN    q ON      ic.id =q.Id 
												  )
												 SELECT  Id As Ids FROM    q";


                        var parentCatInt = ctx.Database.SqlQuery<int>(ff).ToList();

                        if (parentCatInt.Count > 0)
                        {

                            var parentCat = ctx.Categories.Where(x => parentCatInt.Contains(x.Id) && x.ParentCategoryId == 0).Select(x => x.Id).ToList();

                            foreach (var pCat in parentCat)
                                if (uniqueCategories.Contains(pCat) == false)
                                    uniqueCategories.Add(pCat);
                        }
                    }

                    // getting product ids
                    var uniqueProducts = product.Select(x => x.Id).Distinct();

                    var childCats = ctx.Categories.Where(x => x.ParentCategoryId != 0 && uniqueCategories.Contains(x.Id)).ToList();

                    returnModel.Stores = ctx.Stores
                        .IncludeFilter(
                                x => x.Categories.Where(
                                y => uniqueCategories.Contains(y.Id) && uniqueStores.Contains(y.Store_Id) && y.ParentCategoryId == 0)
                                )
                        .IncludeFilter(
                                y => y.Products.Where(
                                z => uniqueProducts.Contains(z.Id) && z.IsDeleted == false)
                                )
                        .Where(y => uniqueStores.Contains(y.Id) && y.IsDeleted == false).ToList();

                    foreach (var store in returnModel.Stores)
                    {
                        foreach (var cat in store.Categories.ToList())
                        {
                            if (cat.ParentCategoryId == 0)
                            {
                                var queryCat = @"WITH    q AS 
                                                (
                                                SELECT  c.Id
                                                FROM    Categories c
                                                WHERE   c.Id = " + cat.Id + " UNION ALL  SELECT  ic.Id FROM    Categories ic JOIN    q ON      ic.ParentCategoryId =q.Id ) SELECT  Id As Ids FROM    q";
                                var CategoryAndSubCategoryIds = ctx.Database.SqlQuery<int>(queryCat).ToList();

                                //Removing itself
                                CategoryAndSubCategoryIds.RemoveAll(x => x == cat.Id);

                                var childCatProducts = ctx.Products.Where(x => CategoryAndSubCategoryIds.Contains(x.Category_Id.Value)).ToList();

                                cat.Products.AddRange(childCatProducts);
                            }
                            else
                                store.Categories.Remove(cat);

                            if (!string.IsNullOrEmpty(Price))
                            {
                                PriceList = Price.Split(',').Select(int.Parse).ToList();

                                if (PriceList.Count == 1)
                                    cat.Products = cat.Products.Where(x => x.Price >= PriceList.First()).ToList();
                                else if (PriceList.Count == 2)
                                    cat.Products = cat.Products.Where(x => x.Price >= PriceList.Min() && x.Price <= PriceList.Max()).ToList();

                            }
                            if (!string.IsNullOrEmpty(ProductNetWeight))
                            {
                                NetWeightList = ProductNetWeight.Split('#').ToList();

                                List<int> UniqueProductSizeIds = new List<int>();
                                foreach (var prod in cat.Products)
                                {
                                    var ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prod.Id && NetWeightList.Contains(x.NetWeight)).ToList();
                                    if (ProductSizes.Count > 0)
                                    {
                                        //prod.ProductSizes.AddRange(ProductSizes);
                                        UniqueProductSizeIds.Add(ProductSizes.FirstOrDefault().Product_Id);
                                    }

                                }
                                cat.Products = cat.Products.Where(x => UniqueProductSizeIds.Contains(x.Id)).ToList();
                            }

                            if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.BestSelling) ;
                            //query += " ORDER BY Stores.Id  DESC ";
                            else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.A2Z)
                                cat.Products = cat.Products.OrderBy(x => x.Name).ToList();
                            else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.Z2A)
                                cat.Products = cat.Products.OrderByDescending(x => x.Name).ToList();
                            else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.Low2High)
                                cat.Products = cat.Products.OrderBy(x => x.Price).ToList();
                            else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.High2Low)
                                cat.Products = cat.Products.OrderByDescending(x => x.Price).ToList();


                            foreach (var prod in cat.Products)
                            {
                                prod.BusinessName = store.BusinessName;
                                prod.BusinessType = store.BusinessType;
                                prod.MinDeliveryCharges = store.MinDeliveryCharges;
                                prod.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == prod.Id).ToList();
                            }
                            //foreach (var item in products)
                            //{
                            //    var store = ctx.Stores.FirstOrDefault(x => x.Id == item.Store_Id);
                            //    item.BusinessName = store.BusinessName;
                            //    item.BusinessType = store.BusinessType;
                            //    item.MinDeliveryCharges = store.MinDeliveryCharges;
                            //    item.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == item.Id).ToList();

                            //}
                        }
                    }
                }
                CustomResponse<AlcoholViewModel> reponse = new CustomResponse<AlcoholViewModel>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = returnModel
                };

                return Ok(reponse);
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }






        //       [HttpGet]
        //       [Route("AlcoholFilterStore")]
        //       public IHttpActionResult AlcoholFilterStore(double latitude, double longitude,int? SortBy = 0, string Country = "", string Price = "", string Size = "")
        //       {
        //           try
        //           {
        //               FilterStoreViewModel model = new FilterStoreViewModel();
        //               AlcoholViewModel returnModel = new AlcoholViewModel();
        //               string[] CountryList;
        //               int[] CountryListInt;

        //               string[] PriceList;
        //               int[] PriceListInt;


        //               string[] SizeList;
        //               int[] SizeListInt;

        //               string ExtendedQuery = "";
        //               string ExtendedWhere = "";

        //               var query = "";


        //               //if (!string.IsNullOrEmpty(Price))
        //               //{
        //               //    PriceList = Price.Split(',');
        //               //    PriceListInt = Array.ConvertAll(PriceList, s => int.Parse(s));

        //               //    ExtendedQuery = "  join Products on Stores.Id=Products.Store_Id ";
        //               //    if (PriceListInt.Count() < 2)
        //               //    {
        //               //        ExtendedWhere = " Products.Price <= " + PriceListInt.FirstOrDefault() + " ";
        //               //    }
        //               //    else
        //               //    {
        //               //        ExtendedWhere = " (Products.Price >= " + PriceListInt.Min() + " OR Products.Price <= " + PriceListInt.Max() + ") ";
        //               //    }
        //               //}


        //               if (latitude != 0 && longitude != 0)
        //               {
        //                   query = @"select DISTINCT Stores.Id,
        //Stores.BusinessType,
        //Stores.Description,
        //Stores.BusinessName,
        //Stores.Latitude,
        //Stores.Longitude,
        //Stores.Open_From,
        //Stores.Open_To,
        //Stores.ImageUrl,
        //Stores.Address,
        //Stores.ContactNumber,
        //Stores.MinDeliveryTime,
        //Stores.MinDeliveryCharges,
        //Stores.MinOrderPrice,
        //Stores.IsDeleted ,AVG(CAST(StoreRatings.Rating AS FLOAT)) AverageRating,Stores.Location.STDistance('POINT(" + longitude + " " + latitude + ")') as Distance  from Stores  left join StoreRatings on Stores.Id = StoreRatings.Store_Id  left join Products on Stores.Id = Products.Store_Id WHERE Stores.BusinessType='Alcohol' AND  ";
        //               }
        //               else
        //               {
        //                   query = @"select DISTINCT Stores.Id,
        //Stores.BusinessType,
        //Stores.Description,
        //Stores.BusinessName,
        //Stores.Latitude,
        //Stores.Longitude,
        //Stores.Open_From,
        //Stores.Open_To,
        //Stores.ImageUrl,
        //Stores.Address,
        //Stores.ContactNumber,
        //Stores.MinDeliveryTime,
        //Stores.MinDeliveryCharges,
        //Stores.MinOrderPrice,
        //Products.Price,
        //Stores.IsDeleted ,AVG(CAST(StoreRatings.Rating AS FLOAT)) AverageRating from Stores
        //LEFT join StoreRatings on Stores.Id = StoreRatings.Store_Id
        //left join Products on Stores.Id = Products.Store_Id
        // Where Stores.BusinessType='Alcohol' AND ";
        //               }





        //               using (DunkeyContext ctx = new DunkeyContext())
        //               {
        //                   if (!string.IsNullOrEmpty(Country))
        //                   {
        //                       CountryList = Country.Split(',');


        //                       for (var i = 1; i <= CountryList.Count(); i++)
        //                       {
        //                           if (i == CountryList.Count())
        //                           {
        //                               query += "Stores.Address LIKE '%" + CountryList[i - 1] + "%' AND ";

        //                           }
        //                           else
        //                           {
        //                               query += "Stores.Address LIKE '%" + CountryList[i - 1] + "%' OR ";
        //                           }
        //                       }
        //                   }




        //                   if (!string.IsNullOrEmpty(Price))
        //                   {
        //                      PriceList = Price.Split(',');
        //                       PriceListInt = Array.ConvertAll(PriceList, s => int.Parse(s));

        //                       if (PriceListInt.Count() == 1)
        //                       {
        //                           query += "  Products.Price > " + PriceListInt[0] + " AND ";
        //                       }
        //                       else if (PriceListInt.Count() == 2)
        //                       {
        //                           query += " (Products.Price >= " + PriceListInt.Min() + " AND Products.Price <= " + PriceListInt.Max() + "  ) AND ";
        //                       }

        //                   }
        //                   if (!string.IsNullOrEmpty(Size))
        //                   {
        //                       SizeList = Size.Split(',');
        //                       SizeListInt = Array.ConvertAll(SizeList, s => int.Parse(s));
        //                   }



        //                   // Group by statements 

        //                   query += @" Stores.IsDeleted='false' group by Stores.Id,
        //                                 Stores.BusinessType,
        //                                Stores.Description,
        //                                Stores.BusinessName,
        //                                Stores.Latitude,
        //                                Stores.Longitude,
        //                                Stores.Open_From,
        //                                Stores.Open_To,
        //                                Stores.ImageUrl,
        //                                Stores.Address,
        //                                Stores.ContactNumber,
        //                                Stores.MinDeliveryTime,
        //                                Stores.MinDeliveryCharges,
        //                                Stores.MinOrderPrice,
        //                                Stores.IsDeleted,
        //                             Products.Price,
        //                                StoreRatings.Rating,
        //                                Stores.Location.STDistance('POINT(" + longitude + " " + latitude + ")')";

        //                   // order by statements 

        //                   if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.BestSelling)
        //                   {
        //                       query += "  ";
        //                   }
        //                   else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.A2Z)
        //                   {
        //                       query += "  Order by Stores.BusinessName ASC ";
        //                   }
        //                   else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.Low2High)
        //                   {
        //                       query += "  Order by Products.Price ASC ";
        //                   }
        //                   else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.High2Low)
        //                   {
        //                       query += "  Order by Products.Price DESC ";
        //                   }
        //                   else
        //                   {
        //                       //query += " ORDER BY MinDeliveryTime ASC";
        //                   }



        //                   var store= ctx.Database.SqlQuery<FilterStores>(query).ToList();

        //                   //var products = ctx.Database.SqlQuery<Product>(query).ToList();
        //                   //Store storee;
        //                   //Category cate;
        //                   //foreach (var product in products)
        //                   //{
        //                   //    storee = ctx.Stores.FirstOrDefault(x => x.Id == product.Store_Id);
        //                   //    cate = ctx.Categories.FirstOrDefault(x => x.Id == product.Category_Id);
        //                   //    cate.Products.Add(product);
        //                   //    storee.Categories.Add(cate);
        //                   //}


        //                   foreach (var SingleStore in store)
        //                   {
        //                       returnModel.Stores.Add(ctx.Stores.Include(x => x.Categories).Include(x => x.Products).FirstOrDefault(y=>y.Id==SingleStore.Id));
        //                   }



        //               }
        //               CustomResponse<AlcoholViewModel> reponse = new CustomResponse<AlcoholViewModel>
        //               {
        //                   Message ="Success",
        //                   StatusCode = (int)HttpStatusCode.OK,
        //                   Result = returnModel
        //               };

        //               return Ok(reponse);

        //           }
        //           catch (Exception ex)
        //           {
        //               return StatusCode(DunkeyDelivery.Utility.LogError(ex));
        //           }

        //       }

        // [HttpGet]
        // [Route("AlcoholFilterStore")]
        // public IHttpActionResult AlcoholFilterStore(double latitude, double longitude, int? SortBy = 0, string Country = "", string Price = "", string Size = "")
        // {
        //     try
        //     {
        //         FilterStoreViewModel model = new FilterStoreViewModel();
        //         AlcoholViewModel returnModel = new AlcoholViewModel();
        //         string[] CountryList;
        //         int[] CountryListInt;

        //         string[] PriceList;
        //         int[] PriceListInt;


        //         string[] SizeList;
        //         int[] SizeListInt;

        //         string ExtendedQuery = "";
        //         string ExtendedWhere = "";


        //         var query = @"SELECT Stores.*, Products.*
        //         FROM Products 
        //         join Stores on Products.Store_Id = Stores.Id
        //LEFT JOIN storeratings 
        //         ON stores.id = storeratings.store_id WHERE stores.businesstype = 'Alcohol' AND   ";

        //         using (DunkeyContext ctx = new DunkeyContext())
        //         {
        //             if (!string.IsNullOrEmpty(Country))
        //             {
        //                 CountryList = Country.Split(',');

        //                 for (var i = 1; i <= CountryList.Count(); i++)
        //                 {
        //                     if (i == CountryList.Count())
        //                     {
        //                         query += "Stores.Address LIKE '%" + CountryList[i - 1] + "%' AND ";

        //                     }
        //                     else
        //                     {
        //                         query += "Stores.Address LIKE '%" + CountryList[i - 1] + "%' OR ";
        //                     }
        //                 }
        //             }




        //             if (!string.IsNullOrEmpty(Price))
        //             {
        //                 PriceList = Price.Split(',');
        //                 PriceListInt = Array.ConvertAll(PriceList, s => int.Parse(s));

        //                 if (PriceListInt.Count() == 1)
        //                 {
        //                     query += "  Products.Price > " + PriceListInt[0] + " AND ";
        //                 }
        //                 else if (PriceListInt.Count() == 2)
        //                 {
        //                     query += " (Products.Price >= " + PriceListInt.Min() + " AND Products.Price <= " + PriceListInt.Max() + "  ) AND ";
        //                 }

        //             }

        //             query += " stores.isdeleted = 'false'  ";
        //             if (!string.IsNullOrEmpty(Size))
        //             {
        //                 //SizeList = Size.Split(',');
        //                 //SizeListInt = Array.ConvertAll(SizeList, s => int.Parse(s));
        //             }


        //             if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.BestSelling)
        //             {
        //                 query += " ORDER BY Stores.Id  DESC ";
        //             }
        //             else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.A2Z)
        //             {
        //                 query += "  Order by Stores.BusinessName ASC ";
        //             }
        //             else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.Low2High)
        //             {
        //                 query += "  Order by Products.Price ASC ";
        //             }
        //             else if (SortBy == (int)DunkeyDelivery.FilterAlcoholSortBy.High2Low)
        //             {
        //                 query += "  Order by Products.Price DESC ";
        //             }
        //             else
        //             {
        //                 query += " ORDER BY Stores.Store_Id DESC";
        //             }

        //             var store = ctx.Database.SqlQuery<FilterStores>(query).ToList();




        //             foreach (var SingleStore in store.Distinct())
        //             {
        //                 var storee = ctx.Stores.Include(x => x.Categories).Include(x => x.Products).FirstOrDefault(y => y.Id == SingleStore.Id && y.BusinessType == "Alcohol");
        //                 if (storee != null)
        //                 {
        //                     returnModel.Stores.Add(storee);

        //                 }
        //             }

        //             foreach (var item in returnModel.Stores.Distinct())
        //             {
        //                 foreach (var subItem in item.Categories)
        //                 {
        //                     foreach (var SubsubItem in subItem.Products)
        //                     {
        //                         SubsubItem.BusinessName = item.BusinessName;
        //                         SubsubItem.MinDeliveryCharges = item.MinDeliveryCharges;
        //                         SubsubItem.BusinessType = item.BusinessType;
        //                         SubsubItem.MinDeliveryTime = item.MinDeliveryTime;
        //                         SubsubItem.MinOrderPrice = item.MinOrderPrice;
        //                     }
        //                 }
        //             }

        //             //returnModel.Stores = returnModel.Stores
        //         }
        //         CustomResponse<AlcoholViewModel> reponse = new CustomResponse<AlcoholViewModel>
        //         {
        //             Message = "Success",
        //             StatusCode = (int)HttpStatusCode.OK,
        //             Result = returnModel
        //         };

        //         return Ok(reponse);

        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(DunkeyDelivery.Utility.LogError(ex));
        //     }

        // }



    }
}
