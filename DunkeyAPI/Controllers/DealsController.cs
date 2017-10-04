using DAL;
using DunkeyAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Deals")]
    public class DealsController : ApiController
    {

        //[HttpGet]
        //[Route("GetOfferPackage")]
        //public IHttpActionResult GetOfferPackage()
        //{
        //    try
        //    {
        //        DunkeyContext ctx = new DunkeyContext();
        //        //var res = ctx.Offer_Packages.Include("Offer_Product").Include("Offer_Packages").ToList();

        //        OfferViewModel model = new OfferViewModel { Offer_Packages = ctx.Offer_Packages.Include(x => x.Package).Include(x => x.Offer.Store).ToList(), Offer_Products = ctx.Offer_Products.Include(x=>x.Product).Include(x=>x.Product.Store).ToList() };

        //        CustomResponse<OfferViewModel> response = new CustomResponse<OfferViewModel>
        //        {
        //            Message = "Success",
        //            StatusCode = (int)HttpStatusCode.OK,
        //            Result = model

        //        };
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(DunkeyDelivery.Utility.LogError(ex));
        //    }

        //}
    }
}
