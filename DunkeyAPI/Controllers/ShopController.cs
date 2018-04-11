using AutoMapper;
using DAL;
using DunkeyAPI.ClientViewModel;
using DunkeyAPI.ExtensionMethods;
using DunkeyAPI.ViewModels;
using DunkeyDelivery;
using GoogleMaps.LocationServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DunkeyAPI.Models
{
    [RoutePrefix("api/Shop")]
    public class ShopController : ApiController
    {

        // [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult CreateShop(ShopBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (ctx.Stores.Any(x => x.BusinessName == model.BusinessName))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "Store already exists" }
                        });
                    }
                    else
                    {
                        var newStoreModel = new Store { BusinessName = model.BusinessName, BusinessType = model.BusinessType, Longitude = model.Longitude, Latitude = model.Latitude, Location = DunkeyDelivery.Utility.CreatePoint(model.Latitude, model.Longitude) };
                        ctx.Stores.Add(newStoreModel);
                        ctx.SaveChanges();
                        CustomResponse<Store> response = new CustomResponse<Store>
                        {
                            Message = Global.SuccessMessage,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = newStoreModel
                        };

                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        [Route("CreateWithImage")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> AddStoreWithImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                ShopBindingModel model = new ShopBindingModel();
                model.BusinessName = httpRequest.Params["BusinessName"];
                model.BusinessType = httpRequest.Params["BusinessType"];
                model.Latitude = Convert.ToDouble(httpRequest.Params["Latitude"]);
                model.Longitude = Convert.ToDouble(httpRequest.Params["Longitude"]);


                Validate(model);

                #region Validations
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multipart data is not included in request." }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image." }
                    });
                }
                #endregion

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (ctx.Stores.Any(x => x.BusinessName == model.BusinessName))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "Store already exists" }
                        });
                    }
                    else
                    {
                        #region ImageSaving
                        var postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB  

                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = Path.GetExtension(postedFile.FileName);
                            var extension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(extension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png." }
                                });
                            }
                            else if (postedFile.ContentLength > MaxContentLength)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto 1 mb." }
                                });
                            }
                            else
                            {
                                int count = 1;
                                fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + postedFile.FileName);

                                while (File.Exists(newFullPath))
                                {
                                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + tempFileName + extension);
                                }
                                postedFile.SaveAs(newFullPath);
                            }
                        }
                        #endregion



                        Store storeModel = new Store
                        {

                            BusinessName = model.BusinessName,
                            BusinessType = model.BusinessType,
                            Latitude = model.Latitude,
                            Longitude = model.Longitude,
                            ImageUrl = DunkeyDelivery.Utility.BaseUrl + ConfigurationManager.AppSettings["StoreImageFolderPath"] + Path.GetFileName(newFullPath),
                            Location = DunkeyDelivery.Utility.CreatePoint(model.Latitude, model.Longitude)
                        };

                        ctx.Stores.Add(storeModel);
                        ctx.SaveChanges();

                        CustomResponse<Store> response = new CustomResponse<Store>
                        {
                            Message = Global.SuccessMessage,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = storeModel
                        };
                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("SearchByType")]
        public IHttpActionResult SearchByType(string Type, int items, int Page)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var res = ctx.Stores.Include("StoreTags").Include(x => x.StoreRatings).Where(x => x.BusinessType == Type && x.IsDeleted == false).OrderBy(x => x.Id).Skip(items * Page).Take(items).ToList();
                    foreach (var storeRating in res)
                    {
                        storeRating.CalculateAverageRating();
                    }
                    res = res.OrderByDescending(x => x.AverageRating).ToList();
                    //var businessTypeTax = ctx.BusinessTypeTax.FirstOrDefault(x => x.BusinessType.Equals(res.BusinessType));

                    //if (businessTypeTax != null)
                    //    res.BusinessTypeTax = businessTypeTax.Tax;

                    var TotalStores = ctx.Stores.Where(x => x.BusinessType == Type).Count();

                    CustomResponse<Shop> response = new CustomResponse<Shop>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new Shop { Store = res, TotalStores = TotalStores }
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetStoreById")]
        public IHttpActionResult GetStoreById(short Id)
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                var res = ctx.Stores.Where(x => x.Id == Id && x.IsDeleted == false).Include("StoreDeliveryHours").Include(x => x.StoreDeliveryTypes).Include(x => x.StoreRatings).First();
                if (res != null)
                {
                    res.CalculateAverageRating();
                }
                var businessTypeTax = ctx.BusinessTypeTax.FirstOrDefault(x => x.BusinessType.Equals(res.BusinessType));

                if (businessTypeTax != null)
                    res.BusinessTypeTax = businessTypeTax.Tax;

                CustomResponse<Store> response = new CustomResponse<Store>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = res
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }
        // <summary > this service is used for both mobile and web </summary >
        [HttpGet]
        [Route("GetStoreCategories")]
        public IHttpActionResult GetStoreCategories(short Store_id)
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                var res = ctx.Categories.Where(x => x.Store_Id == Store_id && x.ParentCategoryId == 0 && x.IsDeleted == false).OrderBy(x => x.Name).ToList();

                CustomResponse<List<Category>> response = new CustomResponse<List<Category>>
                { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = res };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("GetSearchedStores")]
        public IHttpActionResult GetSearchedStores(string search_string, string CategoryType, int ObjectPerPage = 6, int PageNumber = 0)
        {
            try
            {
                Shop res = new Shop();
                DunkeyContext ctx = new DunkeyContext();

                if (string.IsNullOrEmpty(search_string))
                {
                    res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType).OrderBy(x => x.BusinessName).Skip(ObjectPerPage * PageNumber).Take(ObjectPerPage).ToList();
                    res.TotalStores = ctx.Stores.Where(x => x.BusinessType == CategoryType).Count();
                }
                else
                {
                    res.Store = ctx.Stores.Where(x => x.BusinessName.StartsWith(search_string) && x.BusinessType == CategoryType).OrderBy(x => x.BusinessName).Skip(ObjectPerPage * PageNumber).Take(ObjectPerPage).ToList();

                }

                CustomResponse<Shop> response = new CustomResponse<Shop>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = res
                };

                return Ok(response);



            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("GetFilteredStores")]
        public IHttpActionResult GetFilteredStores(string FilterType, string CategoryType, int Items, int Page, DateTime CurrentTime)
        {
            try
            {

                DunkeyContext ctx = new DunkeyContext();
                Shop res = new Shop();
                // var extendedQuery = "";
                //var TotalStores = ctx.Stores.Where(x => x.BusinessType == CategoryType).Count();
                var TotalStores = ctx.Stores.Count(x => x.BusinessType == CategoryType);
                #region ConditionForRatingType
                if (FilterType.Contains("Rating"))
                {
                    res.Store = ctx.Stores.Include(x => x.StoreTags).Where(x => x.BusinessType == CategoryType).OrderByDescending(x => x.AverageRating).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                }
                else if (FilterType.Contains("DeliveryCharges"))
                {
                    res.Store = ctx.Stores.Include(x => x.StoreTags).Where(x => x.BusinessType == CategoryType).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();
                }
                else if (FilterType.Contains("MinOrder"))
                {
                    res.Store = ctx.Stores.Include(x => x.StoreTags).Where(x => x.BusinessType == CategoryType).OrderBy(x => x.MinOrderPrice).Skip(Items * Page).Take(Items).ToList();
                }
                else if (FilterType.Contains("Free delivery"))
                {
                    TotalStores = ctx.Stores.Count(x => x.BusinessType == CategoryType && x.MinDeliveryCharges == 0 || x.MinDeliveryCharges == null);
                    res.Store = ctx.Stores.Include(x => x.StoreTags).Where(x => x.BusinessType == CategoryType && x.MinDeliveryCharges == 0).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                }
                else if (FilterType.Contains("Fast Delivery"))
                {
                    TotalStores = ctx.Stores.Count(x => x.BusinessType == CategoryType);
                    res.Store = ctx.Stores.Include(x => x.StoreTags).Where(x => x.BusinessType == CategoryType).OrderBy(x => x.MinDeliveryTime).ThenBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                }

                else if (FilterType.Contains("Open Restaurants"))
                {
                    #region Condition for day and store timings

                    if (CurrentTime.DayOfWeek.Equals(DayOfWeek.Monday))
                    {
                        res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType && (x.StoreDeliveryHours.Monday_From <= CurrentTime.TimeOfDay && x.StoreDeliveryHours.Monday_To >= CurrentTime.TimeOfDay)).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                    }
                    else if (CurrentTime.DayOfWeek.Equals(DayOfWeek.Tuesday))
                    {
                        res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType && (x.StoreDeliveryHours.Tuesday_From <= CurrentTime.TimeOfDay && x.StoreDeliveryHours.Tuesday_To >= CurrentTime.TimeOfDay)).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                    }
                    else if (CurrentTime.DayOfWeek.Equals(DayOfWeek.Wednesday))
                    {
                        res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType && (x.StoreDeliveryHours.Wednesday_From <= CurrentTime.TimeOfDay && x.StoreDeliveryHours.Wednesday_To >= CurrentTime.TimeOfDay)).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                    }
                    else if (CurrentTime.DayOfWeek.Equals(DayOfWeek.Thursday))
                    {
                        res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType && (x.StoreDeliveryHours.Thursday_From <= CurrentTime.TimeOfDay && x.StoreDeliveryHours.Thursday_To >= CurrentTime.TimeOfDay)).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                    }
                    else if (CurrentTime.DayOfWeek.Equals(DayOfWeek.Friday))
                    {
                        res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType && (x.StoreDeliveryHours.Friday_From <= CurrentTime.TimeOfDay && x.StoreDeliveryHours.Friday_To >= CurrentTime.TimeOfDay)).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                    }
                    else if (CurrentTime.DayOfWeek.Equals(DayOfWeek.Saturday))
                    {
                        res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType && (x.StoreDeliveryHours.Saturday_From <= CurrentTime.TimeOfDay && x.StoreDeliveryHours.Saturday_To >= CurrentTime.TimeOfDay)).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                    }
                    else
                    {
                        res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType && (x.StoreDeliveryHours.Sunday_From <= CurrentTime.TimeOfDay && x.StoreDeliveryHours.Sunday_To >= CurrentTime.TimeOfDay)).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();

                    }
                    #endregion

                }
                else if (FilterType.Contains("All"))
                {
                    res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();
                }
                else if (FilterType.Contains("Online payment avaiable"))
                {
                    res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();
                }
                else
                {
                    // for tags 
                    // var query = "select Stores.*,StoreTags.* from Stores left join StoreTags ON Stores.Id=StoreTags.Store_Id WHERE StoreTags.Tag='"+FilterType+"' AND Stores.BusinessType='"+CategoryType+"' ORDER BY StoreTags.Tag OFFSET 0 ROWS FETCH NEXT 2 ROWS ONLY ";
                    // res.Store = ctx.Database.SqlQuery<Store>(query).ToList();
                    //res.Store = ctx.Stores.Where(x => x.BusinessType == CategoryType).OrderBy(x => x.BusinessName).Skip(Items * Page).Take(Items).ToList();
                    //res.Store = ctx.Stores.Include(x => x.StoreTags.Where(y => y.Tag == "Burger")).Where(x => x.BusinessType == "Food").OrderBy(x => x.BusinessName).ToList();

                    res.Store = ctx.Stores.Include(x => x.StoreTags).Where(x => x.BusinessType == CategoryType && x.StoreTags.Any(y => y.Tag.Contains(FilterType))).ToList();
                }
                #endregion
                res.TotalStores = TotalStores;
                CustomResponse<Shop> response = new CustomResponse<Shop>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = res
                };
                //  response.Result.TotalStores = TotalStores;
                return Ok(response);


            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("GetNearbyStores")]
        public IHttpActionResult GetNearbyStores(string Type, string Address = "", double latitude = 0, double longitude = 0)
        {
            try
            {

                var locationService = new GoogleLocationService("AIzaSyD2qlAy2Quv00vXsc-Ix86WnIXvHmEJlg8");
                var points = locationService.GetLatLongFromAddress(Address);
                if (points == null)
                {

                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Forbidden",
                        StatusCode = (int)HttpStatusCode.Forbidden,
                        Result = new Error { ErrorMessage = "Invalid Address" }
                    });


                }


                var point = DunkeyDelivery.Utility.CreatePoint(points.Latitude, points.Longitude);

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var stores = ctx.Stores.Include("StoreTags").Where(x => x.Location.Distance(point) < Global.NearbyStoreRadius && x.BusinessType == Type).ToList();
                    //var stores = ctx.Stores.Include("StoreTags").Where(x=>x.Address.Contains(Address) && x.BusinessType == Type).ToList();

                    Shop shopViewModel = new Shop
                    {
                        Store = stores


                    };
                    CustomResponse<Shop> reponse = new CustomResponse<Shop>
                    {
                        Message = Global.SuccessMessage,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = shopViewModel
                    };

                    return Ok(reponse);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }
        [HttpGet]
        [Route("GetAllNearbyStores")]
        public IHttpActionResult GetAllNearbyStores(string Address = "", double latitude = 0, double longitude = 0)
        {
            try
            {
                var locationService = new GoogleLocationService();

                var points = locationService.GetLatLongFromAddress(Address);
                if (points == null)
                {

                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Forbidden",
                        StatusCode = (int)HttpStatusCode.Forbidden,
                        Result = new Error { ErrorMessage = "Invalid Address" }
                    });


                }


                var point = DunkeyDelivery.Utility.CreatePoint(points.Latitude, points.Longitude);

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var stores = ctx.Stores.Include("StoreTags").Where(x => x.Location.Distance(point) < Global.NearbyStoreRadius).ToList();
                    //List<ShopViewModel> shopsViews = new List<ShopViewModel>();
                    //foreach (var item in stores)
                    //{
                    //    shopsViews.Add(new ShopViewModel(item));
                    //}
                    Shop shopViewModel = new Shop
                    {
                        Store = stores


                    };
                    CustomResponse<Shop> reponse = new CustomResponse<Shop>
                    {
                        Message = Global.SuccessMessage,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = shopViewModel
                    };

                    return Ok(reponse);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("GetStoreSchedule")]
        public async Task<IHttpActionResult> GetStoreSchedule(int Store_Id)
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                StoreDeliveryTypeList ListOfSchedule = new StoreDeliveryTypeList();
                //ListOfSchedule.StoreScheduleList = ctx.StoreDeliveryTypes.Where(x => x.Store_Id == Store_Id).ToList();
                ListOfSchedule.Store = ctx.Stores.Include(x => x.StoreDeliveryTypes).FirstOrDefault(x => x.Id == Store_Id);
                CustomResponse<StoreDeliveryTypeList> response = new CustomResponse<StoreDeliveryTypeList>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = ListOfSchedule
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllStores")]
        public async Task<IHttpActionResult> GetAllStores()
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();

                CustomResponse<IEnumerable<Store>> response = new CustomResponse<IEnumerable<Store>>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = ctx.Stores.OrderBy(x => x.BusinessName).ToList()
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("SideBarData")]
        public async Task<IHttpActionResult> SideBarData(string CategoryType)
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                SideBar SidebarModel = new SideBar();

                var query = "select StoreTags.Tag ,COUNT(Tag) AS TotalCount from StoreTags join Stores on Stores.Id = StoreTags.Store_Id where Stores.BusinessType = '" + CategoryType + "' AND Stores.IsDeleted='false' GROUP BY StoreTags.Tag ORDER BY TotalCount DESC";
                var cuisines = ctx.Database.SqlQuery<Cuisines>(query).ToList();



                //select StoreTags.Tag ,COUNT(Tag) AS TotalCount from StoreTags GROUP BY StoreTags.Tag ORDER BY TotalCount DESC
                #region Get Store Counts
                SidebarModel.StoreCounts.TotalFoodStores = ctx.Stores.Where(x => x.BusinessType == "Food" && x.IsDeleted == false).Count();
                SidebarModel.StoreCounts.TotalGroceryStores = ctx.Stores.Where(x => x.BusinessType == "Grocery" && x.IsDeleted == false).Count();
                SidebarModel.StoreCounts.TotalAlcoholtores = ctx.Stores.Where(x => x.BusinessType == "Alcohol" && x.IsDeleted == false).Count();
                SidebarModel.StoreCounts.TotalPharmacyStores = ctx.Stores.Where(x => x.BusinessType == "Pharmacy" && x.IsDeleted == false).Count();
                SidebarModel.StoreCounts.TotalLaundryStores = ctx.Stores.Where(x => x.BusinessType == "Laundry" && x.IsDeleted == false).Count();
                SidebarModel.StoreCounts.TotalRetailStores = ctx.Stores.Where(x => x.BusinessType == "Retail" && x.IsDeleted == false).Count();
                #endregion

                var f = Mapper.Map<List<Cuisines>>(cuisines);
                SidebarModel.cuisines = f;
                CustomResponse<SideBar> response = new CustomResponse<SideBar>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = SidebarModel
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        [HttpGet]
        [Route("GetCousines")]
        public async Task<IHttpActionResult> GetCousines()
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                CousineViewModel cousines = new CousineViewModel();

                var query = "select StoreTags.Tag ,COUNT(Tag) AS TotalCount from StoreTags join Stores on Stores.Id = StoreTags.Store_Id where Stores.BusinessType ='Food' AND Stores.IsDeleted='false' GROUP BY StoreTags.Tag ORDER BY TotalCount DESC";
                var cuisines = ctx.Database.SqlQuery<Cuisines>(query).ToList();

                var f = Mapper.Map<List<Cuisines>>(cuisines);
                cousines.cuisines = f;
                CustomResponse<CousineViewModel> response = new CustomResponse<CousineViewModel>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = cousines
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("SubmitStoreReview")]
        public IHttpActionResult SubmitStoreReview(ReviewBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (ctx.StoreRatings.Any(x => x.User_Id == model.User_Id && x.Store_Id == model.Store_Id))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "User already reviews this store." }
                        });
                    }
                    else
                    {
                        var storeRating = new StoreRatings { User_Id = model.User_Id, Rating = model.Rating, Store_Id = model.Store_Id, Feedback = model.Feedback };
                        ctx.StoreRatings.Add(storeRating);
                        ctx.SaveChanges();
                        var latestStoreRating = ctx.StoreRatings.Include(z => z.User).FirstOrDefault(x => x.Id == storeRating.Id);
                        CustomResponse<StoreRatings> response = new CustomResponse<StoreRatings>
                        {
                            Message = Global.SuccessMessage,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = storeRating
                        };

                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("GetStoreReviews")]
        public async Task<IHttpActionResult> GetStoreReviews(int Store_Id)
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                ReviewList returnModel = new ReviewList();
                returnModel.Reviews = ctx.StoreRatings.Include(y => y.User).Where(x => x.Store_Id == Store_Id).ToList();
                CustomResponse<ReviewList> response = new CustomResponse<ReviewList>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = returnModel
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Services For Mobile End Interface 

        [HttpGet]
        [Route("GetStoresByCategories")]
        public IHttpActionResult GetHomeScreenStores(int Category_id, double Lat = 0, double Lng = 0)
        {
            try
            {
                var distance = 0.0;
                List<Store> res = new List<Store>();
                DunkeyContext ctx = new DunkeyContext();
                ClientStoreViewModel response = new ClientStoreViewModel();
                Stores utility = new Stores();
                var Type = "";


                List<string> storetagArray = new List<string>();

                Type = utility.Categories_enum(Category_id);

                if (Lat != 0 && Lng != 0)
                {
                    var point = DunkeyDelivery.Utility.CreatePoint(Lat, Lng);
                    var nearstoreres = ctx.Stores.Where(x => x.BusinessType == Type && x.Location.Distance(point) < Global.NearbyStoreRadius && x.IsDeleted == false).Include(x => x.StoreTags).Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).ToList();

                    foreach (var store in nearstoreres)
                    {
                        store.CalculateAverageRating();
                        distance = store.Location.Distance(point).Value;
                        

                  
                        distance = DunkeyDelivery.Utility.ConvertMeterToMile(distance);
                        var tempDistance = distance.ToString("0.00");
                        distance = Convert.ToDouble(tempDistance);
                        response.NearByStores.Add(new NearByStores(store, distance));

                    }

                    var popularstoreres = ctx.Stores.Where(x => x.BusinessType == Type && x.Location.Distance(point) < Global.NearbyStoreRadius && x.IsDeleted == false).Include(x => x.StoreTags).Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).ToList();

                    foreach (var store in popularstoreres)
                    {
                        store.CalculateAverageRating();
                        distance = store.Location.Distance(point).Value;

                        distance = DunkeyDelivery.Utility.ConvertMeterToMile(distance);
                        var tempDistance = distance.ToString("0.00");
                        distance = Convert.ToDouble(tempDistance);

                        response.PopularStores.Add(new PopularStores(store, distance));
                    }

                    response.PopularStores = response.PopularStores.OrderByDescending(x => x.AverageRating).ToList();

                }


                //res = ctx.Stores.Where(x => x.BusinessType == Type && x.IsDeleted == false).Include(x => x.StoreDeliveryHours).Include(x => x.StoreTags).Include(x => x.StoreRatings).ToArray().ToList();

                //foreach (var store in res)
                //{
                //    // distance = store.Location.Distance(point).Value;
                //    store.CalculateAverageRating();
                //    response.PopularStores.Add(new PopularStores(store));
                //}



                CustomResponse<ClientStoreViewModel> Response = new CustomResponse<ClientStoreViewModel>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = response
                };
                return Ok(Response);

            }

            catch (Exception ex)
            {

                throw;
            }



        }


        //[HttpGet]
        //[Route("StoreCategories")]
        //public IHttpActionResult StoreCategories(short Store_id,int Page=0,int Items=20 )
        //{

        //    try
        //    {
        //        DunkeyContext ctx = new DunkeyContext();
        //        List<CatViewModel> response = new List<CatViewModel>();
        //        //var query = "SELECT Categories.*,IsNull(p.Id,0) as parentId, Isnull(p.Name,'') as parentName FROM Categories LEFT JOIN Categories as p ON(p.Id = Categories.ParentCategoryId) WHERE Categories.Store_Id='"+Store_id+"' ORDER BY Categories.Name OFFSET " + Items*Page+" ROWS FETCH NEXT "+Items+" ROWS ONLY";


        //        var categories = ctx.Database.SqlQuery<CategoryViewModel>(query).ToList();

        //        //  foreach (var cat in categories)
        //        //  {
        //        // if (!string.IsNullOrEmpty(cat.parentName))
        //        //  {


        //        var catModel = new CatViewModel { Categories = categories };
        //        catModel.TotalCategories = ctx.Categories.Where(x => x.Store_Id == Store_id).Count();
        //        foreach (var cat in catModel.Categories.Where(x => x.parentId != 0))
        //        {

        //            catModel.Categories.FirstOrDefault(x => x.Id == cat.parentId).SubCategories.Add(cat);

        //        }

        //        catModel.Categories = catModel.Categories.Where(x => x.parentId == 0).ToList();

        //        //response = Mapper.Map<CatViewModel>(CatViewModel);
        //        // }

        //        //  }


        //        //DunkeyContext ctx = new DunkeyContext();
        //        //var res = ctx.Categories.Where(x => x.Store_Id == Store_id).OrderBy(x => x.Name).ToList();
        //        //ShopCategoriesViewModel categoriesModel = new ShopCategoriesViewModel();
        //        //categoriesModel.Categories = res;


        //        CustomResponse<CatViewModel> returnResponse = new CustomResponse<CatViewModel>
        //        { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = catModel };
        //        return Ok(returnResponse);

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(DunkeyDelivery.Utility.LogError(ex));
        //    }

        //}



        //[HttpGet]
        //[Route("GetStoreAndCategories")]
        //public IHttpActionResult GetStoreAndCategories(int Category_id, double Lat = 0, double Lng = 0)
        //{



        //}


        // used by mobile and website both
        [HttpGet]
        [Route("StoreCategories")]
        public IHttpActionResult StoreCategories(short Store_id, int Page = 0, int Items = 20)
        {

            try
            {
                DunkeyContext ctx = new DunkeyContext();
                CategoriesViewModel cat = new CategoriesViewModel();
                var query = "";
                var parentCats = ctx.Categories.Where(x => x.ParentCategoryId == 0 && x.Store_Id == Store_id).OrderBy(x => x.Id).Skip(Items * Page).Take(Items).ToList();
                //var ParentIds = string.Join(",", parent.Select(x=>x.ParentCategoryId!=0));
                var parentIds = string.Join(",", parentCats.Select(x => x.Id));
                //  var subCategories=ctx.Categories.Where(items=>parentIds.Contains(items.Id)).ToList
                if (string.IsNullOrEmpty(parentIds))
                {

                }
                else
                {
                    query = "SELECT * FROM Categories WHERE Categories.ParentCategoryId IN (" + parentIds + ")";
                    var subCategories = ctx.Database.SqlQuery<SubCategories>(query).ToList();

                    cat.ParentCategories = Mapper.Map<List<ParentCategory>>(parentCats);


                    foreach (var subCat in subCategories)
                    {
                        cat.ParentCategories.FirstOrDefault(x => x.Id == subCat.ParentCategoryId).SubCategories.Add(subCat);
                    }
                    cat.TotalRecords = ctx.Categories.Where(x => x.Store_Id == Store_id && x.ParentCategoryId == 0).Count();
                    cat.DeliveryTypes = ctx.StoreDeliveryTypes.Where(x => x.Store_Id == Store_id).ToList();
                }


                return Ok(new CustomResponse<CategoriesViewModel> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = cat });



            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("GetStoreByIdMobile")]
        public IHttpActionResult GetStoreByIdMobile(short Id, double? latitude = 0, double? longitude = 0)
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                var res = new Store();
                var Distance = 0.0;
                if (latitude != 0 && longitude != 0)
                {
                    var point = DunkeyDelivery.Utility.CreatePoint(latitude.Value, longitude.Value);
                    res = ctx.Stores.Include(z => z.StoreRatings.Select(y => y.User)).Include(z => z.StoreTags).Where(x => x.Id == Id && x.IsDeleted == false).Include("StoreDeliveryHours").First();
                    res.Distance = res.Location.Distance(point).Value;
                }
                else
                {
                    res = ctx.Stores.Include(z => z.StoreRatings.Select(y => y.User)).Include(z => z.StoreTags).Where(x => x.Id == Id && x.IsDeleted == false).Include("StoreDeliveryHours").First();

                }


                var businessTypeTax = ctx.BusinessTypeTax.FirstOrDefault(x => x.BusinessType.Equals(res.BusinessType));

                if (businessTypeTax != null)
                    res.BusinessTypeTax = businessTypeTax.Tax;
                if (res != null)
                {
                    res.SetAverageRating();
                    res.CalculateAllTypesAverageRating();

                    foreach (var item in res.StoreRatings)
                    {
                        var query = "select User_Id, Count(Store_Id) as Count from storeratings group by User_Id";
                        var resp = ctx.Database.SqlQuery<UserRatingCount>(query).ToList();

                        item.User.TotalReviews = resp.FirstOrDefault(x => x.User_Id == item.User.Id).Count;
                        item.User.TotalOrders = ctx.Orders.Count(x => x.User_ID == item.User.Id);
                    }

                }
                CustomResponse<Store> response = new CustomResponse<Store>
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = res
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        [HttpGet]
        [Route("GetNearbyStoresMobile")]
        public IHttpActionResult GetNearbyStoresMobile(string Type, double latitude = 0, double longitude = 0, int? Page = 0, int? Items = 10)
        {
            try
            {

                var point = DunkeyDelivery.Utility.CreatePoint(latitude, longitude);


                using (DunkeyContext ctx = new DunkeyContext())
                {
                    Shop shopViewModel = new Shop();
                    List<Store> stores = new List<Store>();
                    if (!string.IsNullOrEmpty(Type))
                    {

                        if (point != null)
                        {
                            stores = ctx.Stores.Include(x => x.StoreTags).Include(x => x.StoreRatings).Include(x => x.StoreDeliveryHours).Where(x => x.Location.Distance(point) < Global.NearbyStoreRadius && x.BusinessType == Type).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                            shopViewModel.TotalStores = ctx.Stores.Count(x => x.Location.Distance(point) < Global.NearbyStoreRadius && x.BusinessType == Type);
                        }
                        else
                        {
                            stores = ctx.Stores.Include(x => x.StoreTags).Include(x => x.StoreRatings).Include(x => x.StoreDeliveryHours).Where(x => x.BusinessType == Type).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                            shopViewModel.TotalStores = ctx.Stores.Count(x => x.BusinessType == Type);
                        }

                    }
                    else
                    {
                        stores = ctx.Stores.Include(x => x.StoreTags).Include(x => x.StoreRatings).Include(x => x.StoreDeliveryHours).OrderBy(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        shopViewModel.TotalStores = ctx.Stores.Count();

                    }


                    if (stores.Count > 0)
                    {

                        foreach (var store in stores)
                        {
                            if (latitude != 0 && longitude != 0)
                            {
                                store.Distance = store.Location.Distance(point).Value;
                            }
                            store.CalculateAverageRating();
                            store.CalculateAllTypesAverageRating();
                        }


                    }



                    shopViewModel.Store = stores;


                    CustomResponse<Shop> reponse = new CustomResponse<Shop>
                    {
                        Message = Global.SuccessMessage,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = shopViewModel
                    };

                    return Ok(reponse);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("FilterStore")]
        public IHttpActionResult FilterStore(string CategoryName = "", int SortBy = 0, int Rating = 0, int MinDeliveryTime = 0, string PriceRanges = "", decimal MinDeliveryCharges = 0, bool IsSpecial = false, bool IsFreeDelivery = false, bool IsNewRestaurants = false, string Cuisines = "", double latitude = 0, double longitude = 0)
        {
            try
            {
                FilterStoreViewModel model = new FilterStoreViewModel();
                // for products
                var ExtendedQuery = "";
                var ExtendedWhere = "";
                //  ----

                // for price ranges
                string[] PriceRangesString;
                int[] PriceRangesInt = new int[0];
                // --------
                var query = "";
                if (!string.IsNullOrEmpty(PriceRanges))
                {
                    PriceRangesString = PriceRanges.Split(',');
                    PriceRangesInt = Array.ConvertAll(PriceRangesString, s => int.Parse(s));

                    ExtendedQuery = "  join Products on Stores.Id=Products.Store_Id ";
                    if (PriceRangesInt.Count() < 2)
                    {
                        ExtendedWhere = " Products.Price <= " + PriceRangesInt.FirstOrDefault() + " ";
                    }
                    else
                    {
                        ExtendedWhere = "(Products.Price >= " + PriceRangesInt.Min() + " OR Products.Price <= " + PriceRangesInt.Max() + ") ";
                    }
                }

                if (latitude != 0 && longitude != 0)
                {
                    //   query = "SELECT Stores.*,Stores.Location.STDistance('POINT(" + longitude + " " + latitude + ")') as Distance ,(SELECT AVG(CAST(StoreRatings.Rating AS FLOAT)) From StoreRatings WHERE StoreRatings.Store_Id=Stores.Id)as AverageRating From Stores WHERE ";

                    query = @"select Stores.Id,
 Stores.BusinessType,
 Stores.Description,
 Stores.BusinessName,
 Stores.Latitude,
 Stores.Longitude,
 Stores.Open_From,
 Stores.Open_To,
 Stores.ImageUrl,
 Stores.Address,
 Stores.ContactNumber,
 Stores.MinDeliveryTime,
 Stores.MinDeliveryCharges,
 Stores.MinOrderPrice,
 Stores.IsDeleted ,AVG(CAST(StoreRatings.Rating AS FLOAT)) AverageRating,Stores.Location.STDistance('POINT(" + longitude + " " + latitude + ")') as Distance  from Stores left join StoreRatings on Stores.Id = StoreRatings.Store_Id WHERE ";

                }
                else
                {
                    query = @"select Stores.Id,
 Stores.BusinessType,
 Stores.Description,
 Stores.BusinessName,
 Stores.Latitude,
 Stores.Longitude,
 Stores.Open_From,
 Stores.Open_To,
 Stores.ImageUrl,
 Stores.Address,
 Stores.ContactNumber,
 Stores.MinDeliveryTime,
 Stores.MinDeliveryCharges,
 Stores.MinOrderPrice,
 Stores.IsDeleted ,AVG(CAST(StoreRatings.Rating AS FLOAT)) AverageRating from Stores
 left join StoreRatings on Stores.Id = StoreRatings.Store_Id WHERE ";
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {

                    if (MinDeliveryTime != 0)
                    {
                        query += " Stores.MinDeliveryTime <= " + MinDeliveryTime + " AND ";
                    }
                    if (MinDeliveryCharges != 0)
                    {
                        query += " Stores.MinDeliveryCharges <= " + MinDeliveryCharges + " AND ";
                    }
                    if (MinDeliveryCharges == 0 && IsFreeDelivery == true)
                    {
                        query += " Stores.MinDeliveryCharges = 0  AND ";
                    }
                    if (IsNewRestaurants)
                    {
                        //query += "Stores.MinDeliveryCharges = 0 AND";
                    }



                    query += " Stores.IsDeleted='false' AND Stores.BusinessType='" + CategoryName + "' ";


                    query += @" group by Stores.Id,
                                  Stores.BusinessType,
                                 Stores.Description,
                                 Stores.BusinessName,
                                 Stores.Latitude,
                                 Stores.Longitude,
                                 Stores.Open_From,
                                 Stores.Open_To,
                                 Stores.ImageUrl,
                                 Stores.Address,
                                 Stores.ContactNumber,
                                 Stores.MinDeliveryTime,
                                 Stores.MinDeliveryCharges,
                                 Stores.MinOrderPrice,
                                 Stores.IsDeleted,
                                 StoreRatings.Rating";

                    if (latitude != 0 && longitude != 0)
                    {
                        query += ",Stores.Location.STDistance('POINT(" + longitude + " " + latitude + ")')";
                    }

                    if (Rating != 0)
                    {
                        query += "  having AVG(StoreRatings.Rating) = " + Rating + " ";
                    }

                    if (SortBy == (int)FilterSortBy.DeliveryTime)
                    {
                        query += " ORDER BY MinDeliveryTime ASC";
                    }
                    else if (SortBy == (int)FilterSortBy.Distance)
                    {
                        query += " ORDER BY Distance ASC";
                    }
                    else if (SortBy == (int)FilterSortBy.MinDelivery && latitude != 0 && longitude != 0)
                    {
                        query += " ORDER BY MinDeliveryCharges ASC";
                    }
                    else if (SortBy == (int)FilterSortBy.Price)
                    {
                        //query += " ORDER BY MinDeliveryTime ASC";
                    }
                    else if (SortBy == (int)FilterSortBy.Rating)
                    {
                        query += " ORDER BY AverageRating DESC";
                    }
                    else if (SortBy == (int)FilterSortBy.AtoZ)
                    {
                        query += " ORDER BY Stores.BusinessName ASC";
                    }
                    else
                    {
                        //query += " ORDER BY MinDeliveryTime ASC";
                    }
                    model.NearByStores = ctx.Database.SqlQuery<FilterStores>(query).ToList();

                    foreach (var item in model.NearByStores)
                    {
                        item.StoreRatings = ctx.StoreRatings.Where(x => x.Store_Id == item.Id).ToList();
                        item.CalculateCustomStoreAverageRating();


                    }
                    model.PopularStores = model.NearByStores.OrderByDescending(x => x.AverageRating).ToList();
                    if (IsNewRestaurants != true || IsFreeDelivery != true)
                    {

                        if (IsNewRestaurants)
                        {
                            model.NearByStores = model.NearByStores.Where(x => x.AverageRating == 0).ToList();
                            model.PopularStores = model.PopularStores.Where(x => x.AverageRating == 0).ToList();

                        }
                        if (IsFreeDelivery)
                        {
                            model.NearByStores = model.NearByStores.Where(x => x.MinDeliveryCharges == 0).ToList();
                            model.PopularStores = model.PopularStores.Where(x => x.MinDeliveryCharges == 0).ToList();
                        }
                    }else
                    {
                        model.NearByStores = model.NearByStores.Where(x => x.MinDeliveryCharges == 0 && x.AverageRating==0).ToList();
                        model.PopularStores = model.PopularStores.Where(x => x.MinDeliveryCharges == 0 && x.AverageRating == 0).ToList();
                    }


                    CustomResponse<FilterStoreViewModel> reponse = new CustomResponse<FilterStoreViewModel>
                    {
                        Message = Global.SuccessMessage,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };

                    return Ok(reponse);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        //[HttpGet]
        //[Route("GetStoreByIdMobile")]
        //public IHttpActionResult GetStoreByIdMobile(short Id)
        //{
        //    try
        //    {
        //        DunkeyContext ctx = new DunkeyContext();
        //        var res = ctx.Stores.Where(x => x.Id == Id && x.IsDeleted == false).Include("StoreDeliveryHours").Include(y=>y.).First();

        //        var businessTypeTax = ctx.BusinessTypeTax.FirstOrDefault(x => x.BusinessType.Equals(res.BusinessType));

        //        if (businessTypeTax != null)
        //            res.BusinessTypeTax = businessTypeTax.Tax;

        //        CustomResponse<Store> response = new CustomResponse<Store>
        //        {
        //            Message = "Success",
        //            StatusCode = (int)HttpStatusCode.OK,
        //            Result = res
        //        };
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(DunkeyDelivery.Utility.LogError(ex));
        //    }

        //}






        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }





}