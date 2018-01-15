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

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Alcohol")]
    public class AlcoholController : ApiController
    {
        [HttpGet]
        [Route("AlcoholHomeScreen")]
        public async Task<IHttpActionResult> AlcoholHomeScreen(int? Page=0,int? Items=10)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {

                    AlcoholViewModel model = new AlcoholViewModel();

                    var Stores = ctx.Stores.Include(x=>x.Categories).Include(x=>x.Products).Where(x => x.BusinessType == Utility.Global.Constants.Alcohol && x.IsDeleted==false).OrderBy(x=>x.Id).Skip(Page.Value*Items.Value).Take(Items.Value).ToList();
                    model.Stores = Stores;

                    foreach (var store in model.Stores)
                    {
                        foreach (var cat in store.Categories)
                        {
                            store.Categories = store.Categories.Where(x => x.IsDeleted == false && x.Store_Id==store.Id).ToList();

                            foreach (var prod in cat.Products)
                            {
                                cat.Products = cat.Products.Where(x => x.IsDeleted == false && x.Category_Id==cat.Id).ToList();
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


    }
}
