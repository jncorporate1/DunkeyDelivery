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
                using (DunkeyContext ctx = new DunkeyContext())
                {


                    AlcoholViewModel model = new AlcoholViewModel();
                    var Stores = new List<Store>();
                    string[] StoreIdsString;
                    int[] StoreIdsInt = new int[0];
                    var point = DunkeyDelivery.Utility.CreatePoint(latitude, longitude);

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
                        Stores = ctx.Stores.Include(x => x.Categories).Include(x => x.Products).Include(x => x.StoreTags).Where(x => x.Location.Distance(point) < DunkeyDelivery.Global.NearbyStoreRadius && x.BusinessType == Utility.Global.Constants.Alcohol && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                    }


                    if (Stores.Count > 0)
                    {
                        foreach (var store in Stores)
                        {
                            if (store.Categories.Any(x => (x.Name.Contains("Wine") || x.Name.Contains("Liquor")) && x.ParentCategoryId == 0))
                            {
                                foreach (var cat in store.Categories)
                                {
                                    store.Categories = store.Categories.Where(x => x.IsDeleted == false && x.Store_Id == store.Id && x.ParentCategoryId == 0).ToList();

                                    foreach (var prod in cat.Products)
                                    {
                                        prod.BusinessName = store.BusinessName;
                                        cat.Products = cat.Products.Where(x => x.IsDeleted == false && x.Category_Id == cat.Id).ToList();
                                    }
                                }
                                store.Distance = store.Location.Distance(point).Value;
                                store.CalculateAverageRating();
                                model.Stores.Add(store);
                            }

                        }

                    }
                    return Ok(new CustomResponse<AlcoholViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("AlcoholStoreCategoryDetails")]
        public async Task<IHttpActionResult> AlcoholStoreCategoryDetails(int Store_Id, int? Category_ParentId = 0,string CategoryName="", int? Page = 0, int? Items = 10)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {

                    AlcoholStoreCategories model = new AlcoholStoreCategories();
                    AlcoholStoreParentCategories responseModel = new AlcoholStoreParentCategories();
                    //AlcoholChildCategories childModel = new AlcoholChildCategories();
                    var ParentIds = new List<int>();

                    var WineParentId = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Name.Contains("Wine") && x.Store_Id==Store_Id  && x.IsDeleted == false).FirstOrDefault();
                    var LiquorParentId = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Store_Id == Store_Id && x.Name.Contains("Liquor") && x.IsDeleted == false).FirstOrDefault();
                    var BeerParentId = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Store_Id == Store_Id && x.Name.Contains("Beer") && x.IsDeleted == false).FirstOrDefault();

                    if (Category_ParentId.Value == 0)
                    {

                        responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.ParentCategoryId.Value== WineParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x =>x.Store.Id == Store_Id && x.ParentCategoryId.Value == LiquorParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value ==BeerParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        return Ok(new CustomResponse<AlcoholStoreParentCategories> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CategoryName))
                        {
                            if (CategoryName.Contains("Wine"))
                            {
                                responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == LiquorParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == BeerParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                            }
                            else if (CategoryName.Contains("Liquor"))
                            {
                                responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value== WineParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value ==BeerParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                    
                            }else
                            {
                                responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == WineParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == LiquorParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                                responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                            }
                        }
                        //childModel.Categories = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId == Category_ParentId.Value && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        return Ok(new CustomResponse<AlcoholStoreParentCategories> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });

                    }


                }
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
                        store.CalculateAverageRating();
                    }


                    return Ok(new CustomResponse<AlcoholViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }



    }
}
