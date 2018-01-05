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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                    var res = ctx.Stores.Include("StoreTags").Where(x => x.BusinessType == Type && x.IsDeleted == false).OrderBy(x => x.BusinessName).Skip(items * Page).Take(items).ToList();

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
                var res = ctx.Stores.Where(x => x.Id == Id && x.IsDeleted == false).Include("StoreDeliveryHours").First();

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
                var res = ctx.Categories.Where(x => x.Store_Id == Store_id && x.ParentCategoryId == 0 && x.IsDeleted==false).OrderBy(x => x.Name).ToList();

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
                    if (ctx.StoreRatings.Any(x => x.User_Id== model.User_Id && x.Store_Id==model.Store_Id))
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
                        var storeRating = new StoreRatings { User_Id=model.User_Id,Rating=model.Rating,Store_Id=model.Store_Id,Feedback=model.Feedback };
                        ctx.StoreRatings.Add(storeRating);
                        ctx.SaveChanges();
                        var latestStoreRating = ctx.StoreRatings.Include(z=>z.User).FirstOrDefault(x=>x.Id==storeRating.Id);
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
                returnModel.Reviews = ctx.StoreRatings.Include(y=>y.User).Where(x => x.Store_Id == Store_Id).ToList(); 
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


                #region Enum To Text Based Condition ( Commented ) 


                //switch (Category_id)
                //{
                //    case ((int)Stores.Categories.Food):
                //        Type = "Food";
                //        break;
                //    case ((int)Stores.Categories.Alcohol):
                //        Type = "Alcohol";
                //        break;
                //    case ((int)Stores.Categories.Grocery):
                //        Type = "Grocery";
                //        break;
                //    case ((int)Stores.Categories.Laundry):
                //        Type = "Laundry";
                //        break;
                //    case ((int)Stores.Categories.Pharmacy):
                //        Type = "Pharmacy";
                //        break;
                //    case ((int)Stores.Categories.Retail):
                //        Type = "Retail";
                //        break;
                //    default:
                //        break;

                //}
                #endregion

                Type = utility.Categories_enum(Category_id);

                if (Lat != 0 && Lng != 0)
                {
                    var point = DunkeyDelivery.Utility.CreatePoint(Lat, Lng);
                    res = ctx.Stores.Where(x => x.BusinessType == Type && x.Location.Distance(point) < Global.NearbyStoreRadius).Include(x => x.StoreTags).Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).ToList();

                    foreach (var store in res)
                    {
                        distance = store.Location.Distance(point).Value;
                        response.NearByStores.Add(new NearByStores(store, distance));

                    }

                }


                res = ctx.Stores.Where(x => x.BusinessType == Type).Include(x => x.StoreDeliveryHours).Include(x => x.StoreTags).Include(x => x.StoreRatings).ToArray().ToList();

                foreach (var store in res)
                {
                    // distance = store.Location.Distance(point).Value;
                    response.PopularStores.Add(new PopularStores(store));
                }



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
        public IHttpActionResult GetStoreByIdMobile(short Id)
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                var res = ctx.Stores.Include(z=>z.StoreRatings.Select(y=>y.User)).Include(z=>z.StoreTags).Where(x => x.Id == Id && x.IsDeleted == false).Include("StoreDeliveryHours").First();

                var businessTypeTax = ctx.BusinessTypeTax.FirstOrDefault(x => x.BusinessType.Equals(res.BusinessType));

                if (businessTypeTax != null)
                    res.BusinessTypeTax = businessTypeTax.Tax;

                res.CalculateAllTypesAverageRating();
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