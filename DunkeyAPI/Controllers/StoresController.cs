using DAL;
using DunkeyAPI.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using System.Data.Entity;
using static DunkeyAPI.Utility.Global;
using DunkeyAPI.ExtensionMethods;
using DunkeyDelivery;

namespace DunkeyAPI.Controllers
{
    // services for admin panel
    [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
    [RoutePrefix("api")]
    public class StoresController : ApiController
    {
        /// <summary>
        /// Get All Stores
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllStores")]
        public async Task<IHttpActionResult> GetAllStores()
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var stores = ctx.Stores.Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).ToList();

                    foreach (var store in stores)
                    {
                        store.CalculateAverageRating();
                    }
                    CustomResponse<IEnumerable<Store>> response = new CustomResponse<IEnumerable<Store>>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = stores
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Get Stores by Franchisor Id
        /// </summary>
        /// <param name="FranchisorId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllStoresByFranchisorId")]
        public async Task<IHttpActionResult> GetAllStoresByFranchisorId(int FranchisorId)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    CustomResponse<IEnumerable<Store>> response = new CustomResponse<IEnumerable<Store>>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = ctx.Stores.ToList()
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Get Stores Count
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStoresCount")]
        public async Task<IHttpActionResult> GetStoresCount()
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    StoreCountViewModel model = new StoreCountViewModel { TotalStores = ctx.Stores.Count() };
                    CustomResponse<StoreCountViewModel> response = new CustomResponse<StoreCountViewModel>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Get Nearby Stores, Default radius is 50 miles. To get all stores, give long and lat = 0
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        [Route("GetNearbyStores")]
        public async Task<IHttpActionResult> GetNearbyStores(double longitude, double latitude, int PageSize, int PageNo, string filterTypes)
        {
            try
            {
                var point = DunkeyDelivery.Utility.CreatePoint(latitude, longitude);

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    StoresViewModel storesViewModel = new StoresViewModel();
                    if (longitude == 0 && latitude == 0)
                    {
                        storesViewModel.Count = ctx.Stores.Count(x => x.IsDeleted == false);
                        storesViewModel.Stores = ctx.Stores.Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }
                    else
                    {
                        storesViewModel.Count = ctx.Stores.Where(x => x.Location.Distance(point) < DunkeyDelivery.Global.NearbyStoreRadius && x.IsDeleted == false).Count();
                        storesViewModel.Stores = ctx.Stores.Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).Where(x => x.Location.Distance(point) < DunkeyDelivery.Global.NearbyStoreRadius && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }

                    foreach (var store in storesViewModel.Stores)
                    {
                        store.CalculateAverageRating();
                    }

                    CustomResponse<StoresViewModel> reponse = new CustomResponse<StoresViewModel>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = storesViewModel
                    };

                    return Ok(reponse);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }
    }
}
