﻿using DAL;
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
using static DunkeyDelivery.Utility;
using DunkeyAPI.Models;

namespace DunkeyAPI.Controllers
{
   // [Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
    [RoutePrefix("api")]
    public class CommonController : ApiController
    {
        [HttpGet]
        [Route("GetEntityById")]
        public async Task<IHttpActionResult> GetEntityById(int EntityType, int Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    switch (EntityType)
                    {
                        case (int)DunkeyEntityTypes.Product:
                            var product = ctx.Products.Include(x => x.ProductSizes).FirstOrDefault(x => x.Id == Id && x.IsDeleted == false);
                            product.ProductSizes = ctx.ProductSizes.Where(x => x.Product_Id == product.Id).ToList();
                            return Ok(new CustomResponse<Product> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = product });

                        case (int)DunkeyEntityTypes.Category:
                            return Ok(new CustomResponse<Category> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Categories.FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });

                        case (int)DunkeyEntityTypes.Store:
                            return Ok(new CustomResponse<Store> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Stores.Include(x => x.StoreDeliveryHours).Include(x=>x.StoreDeliveryTypes).FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });

                        case (int)DunkeyEntityTypes.Package:
                            return Ok(new CustomResponse<Package> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Packages.FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });

                        case (int)DunkeyEntityTypes.Admin:
                             return Ok(new CustomResponse<DAL.Admin> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Admins.FirstOrDefault(x => x.Id == Id) });
                      
                        case (int)DunkeyEntityTypes.Offer:
                            return Ok(new CustomResponse<DAL.Offer> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ctx.Offers.FirstOrDefault(x => x.Id == Id && x.IsDeleted == false) });
                        case (int)DunkeyEntityTypes.Reward:
                            var resp = ctx.RewardMilestones.Include(x => x.RewardPrizes).FirstOrDefault(x => x.Id == Id && x.IsDeleted == false);
                            resp.RewardPrizes = ctx.RewardPrize.FirstOrDefault(x => x.Id == resp.RewardPrize_Id);
                            if (resp.RewardPrizes == null)
                                resp.RewardPrizes = new RewardPrize();

                            return Ok(new CustomResponse<DAL.RewardMilestones> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = resp });

                        default:
                            return Ok(new CustomResponse<Error> { Message = ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid entity type" } });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetFAQs")]
        public async Task<IHttpActionResult> GetFAQs(string Type,int? Page=0,int? Items=10000)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    
                    var FAQ =new FAQViewModel();
                    if (string.IsNullOrEmpty(Type))
                    {
                        FAQ.FAQs = ctx.FAQ.Where(x=>x.isDeleted==false).OrderByDescending(x=>x.Id).Skip(Page.Value*Items.Value).Take(Items.Value).ToList();
                        FAQ.TotalRecords = ctx.FAQ.Count(x => x.isDeleted == false);
                    }
                    else
                    {
                        FAQ.FAQs = ctx.FAQ.Where(x => x.Type == Type && x.isDeleted == false).OrderByDescending(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        FAQ.TotalRecords = ctx.FAQ.Count(x => x.Type == Type && x.isDeleted == false);
                    }

                    return Ok(new CustomResponse<FAQViewModel> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = FAQ });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetFAQsById")]
        public async Task<IHttpActionResult> GetFAQsById(int Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var FAQ = new FAQ();
                    FAQ = ctx.FAQ.FirstOrDefault(x =>x.Id==Id && x.isDeleted == false);
                    return Ok(new CustomResponse<FAQ> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = FAQ });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }
    }
}
