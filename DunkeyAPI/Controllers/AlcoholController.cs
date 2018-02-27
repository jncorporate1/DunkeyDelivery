﻿using DAL;
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
                    AlcoholViewModel ReturnModel = new AlcoholViewModel();

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

                            if (store.Categories.Any(x => (x.Name.Contains("Wine") || x.Name.Contains("Liquor") || x.Name.Contains("Beer")) && x.ParentCategoryId == 0))
                            {
                                foreach (var cat in store.Categories)
                                {
                                    store.Categories = store.Categories.Where(x => x.IsDeleted == false && x.Store_Id == store.Id && x.ParentCategoryId == 0).ToList();

                                    foreach (var prod in cat.Products)
                                    {
                                        prod.BusinessName = store.BusinessName;
                                        prod.BusinessType = store.BusinessType;
                                        prod.MinDeliveryCharges = store.MinDeliveryCharges;
                                        prod.MinDeliveryTime = store.MinDeliveryTime;
                                        prod.MinOrderPrice = store.MinOrderPrice;
                                        cat.Products = cat.Products.Where(x => x.IsDeleted == false && x.Category_Id == cat.Id).ToList();
                                    }
                                }
                                //store.Distance = store.Location.Distance(point).Value;
                                store.Distance = 0;
                                store.CalculateAverageRating();
                                model.Stores.Add(store);
                            }
                        }
                        //Todo
                        var beerStore = model.Stores.FirstOrDefault(x => x.Categories.Any(y => y.Name == "Beer"));
                        if (beerStore != null)
                        {
                            beerStore.CategoryType = (int)DunkeyDelivery.Stores.AlcoholCategoryTypes.Beer;
                        }
                        model.Stores = model.Stores.OrderBy(x => x.CategoryType).ToList();
                        //model.Stores = model.Stores.OrderBy(x => x.Categories).ToList();
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
        public async Task<IHttpActionResult> AlcoholStoreCategoryDetails(int Store_Id, int? Category_Id = 0, int? Category_ParentId = 0, string CategoryName = "", int? Page = 0, int? Items = 10,int? Type_Id=0)
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

                    var WineParentId = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Name.Contains("Wine") && x.Store_Id == Store_Id && x.IsDeleted == false).FirstOrDefault();
                    var LiquorParentId = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Store_Id == Store_Id && x.Name.Contains("Liquor") && x.IsDeleted == false).FirstOrDefault();
                    var BeerParentId = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Store_Id == Store_Id && x.Name.Contains("Beer") && x.IsDeleted == false).FirstOrDefault();

                    if (Category_ParentId.Value == 0)
                    {
                        if (WineParentId != null)
                        {
                            responseModel.Categories.Wine = ctx.Categories.Include(x => x.Products).Where(x => x.ParentCategoryId.Value == WineParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        }
                        if (LiquorParentId != null)
                        {
                            responseModel.Categories.Liquor = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == LiquorParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        }
                        if (BeerParentId != null)
                        {
                            responseModel.Categories.Beer = ctx.Categories.Include(x => x.Products).Where(x => x.Store.Id == Store_Id && x.ParentCategoryId.Value == BeerParentId.Id && x.IsDeleted == false).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
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
                        return Ok(new CustomResponse<AlcoholStoreParentCategories> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });

                    }else if (!string.IsNullOrEmpty(CategoryName) && Type_Id != 0)
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

                        return Ok(new CustomResponse<AlcoholStoreParentCategories> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });


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
                        //    store.Distance = store.Location.Distance(point).Value;
                        store.Distance = 0;
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

        [HttpGet]
        [Route("AlcoholFilterStore")]
        public IHttpActionResult AlcoholFilterStore(double latitude, double longitude, int? SortBy = 0, string Country = "", string Price = "", string Size = "",int? Type=0)
        {
            try
            {
                FilterStoreViewModel model = new FilterStoreViewModel();
                AlcoholViewModel returnModel = new AlcoholViewModel();
                string[] CountryList;
                int[] CountryListInt;

                string[] PriceList;
                int[] PriceListInt;


                string[] SizeList;
                int[] SizeListInt;

                string ExtendedQuery = "";
                string ExtendedWhere = "";


                var query = @"SELECT  Products.*
                FROM Products 
                join Stores on Products.Store_Id = Stores.Id
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



                    var store = ctx.Database.SqlQuery<FilterStores>(query).ToList();

                    //var products = ctx.Database.SqlQuery<Product>(query).ToList();
                    //Store storee;
                    //Category cate;
                    //foreach (var product in products)
                    //{
                    //    storee = ctx.Stores.FirstOrDefault(x => x.Id == product.Store_Id);
                    //    cate = ctx.Categories.FirstOrDefault(x => x.Id == product.Category_Id);
                    //    cate.Products.Add(product);
                    //    storee.Categories.Add(cate);
                    //}


                    foreach (var SingleStore in store)
                    {
                        var storee=ctx.Stores.Include(x => x.Categories).Include(x => x.Products).FirstOrDefault(y => y.Id == SingleStore.Id && y.BusinessType == "Alcohol");
                        if(storee != null)
                        {
                            returnModel.Stores.Add(storee);

                        }
                    }

                    foreach (var item in returnModel.Stores)
                    {
                        foreach (var subItem in item.Categories)
                        {
                            foreach (var SubsubItem in subItem.Products)
                            {
                                SubsubItem.BusinessName = item.BusinessName;
                                SubsubItem.MinDeliveryCharges = item.MinDeliveryCharges;
                                SubsubItem.BusinessType = item.BusinessType;
                                SubsubItem.MinDeliveryTime = item.MinDeliveryTime;
                                SubsubItem.MinOrderPrice = item.MinOrderPrice;
                            }
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



    }
}
