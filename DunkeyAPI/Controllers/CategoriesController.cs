using DAL;
using DunkeyAPI.Models;
using DunkeyAPI.ViewModels;
using DunkeyDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using AutoMapper;
using System.Threading.Tasks;
using static DunkeyAPI.Utility.Global;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Categories")]
    public class CategoriesController : ApiController
    {
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(ShopCategoriesBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (ctx.Categories.Any(x => x.Store_Id == model.Store_Id && x.Name == model.Name))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "Category already exists under same store" }
                        });
                    }
                    else
                    {
                        var newCategoryModel = new Category { Name = model.Name, Description = model.Description, Status = model.Status, Store_Id = model.Store_Id, ParentCategoryId = model.ParentCategoryId };
                        ctx.Categories.Add(newCategoryModel);
                        ctx.SaveChanges();
                        CustomResponse<Category> response = new CustomResponse<Category>
                        {
                            Message = Global.SuccessMessage,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = newCategoryModel
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
        [Route("GetCategoryProducts")]
        public IHttpActionResult GetStoreCategories(short Store_id)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var catQuery = @"select * from categories where isdeleted = 0 and Store_Id = " + Store_id;

                    var categories = ctx.Database.SqlQuery<categoryViewModel>(catQuery).ToList();

                    var catIds = string.Join(",", categories.Select(x => x.Id.ToString()));
                    var productQuery = "";
                    if (string.IsNullOrEmpty(catIds))
                    {
                        productQuery = @"select * from products where isdeleted = 0";
                    }
                    else
                    {
                        productQuery = @"select * from products where isdeleted = 0 and category_id in (" + catIds + ")";

                    }

                    var products = ctx.Database.SqlQuery<Product>(productQuery).ToList();
                    if (categories.Count != 0)
                    {
                        foreach (var product in products)
                        {
                            categories.FirstOrDefault(x => x.Id == product.Category_Id).Products.Add(product);
                        }
                    }
                    CustomResponse<List<categoryViewModel>> response = new CustomResponse<List<categoryViewModel>>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = categories
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
        [Route("GetSubCategories")]
        public IHttpActionResult GetSubCategories(short Category_id)
        {
            try
            {
                DunkeyContext ctx = new DunkeyContext();
                List<SubCategories> catModel = new List<SubCategories>();
                var res = ctx.Categories.Where(x => x.ParentCategoryId == Category_id && x.IsDeleted == false).ToList();

                var f = Mapper.Map<List<SubCategories>>(res);
                catModel = f;
                CustomResponse<List<SubCategories>> response = new CustomResponse<List<SubCategories>>
                { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = catModel };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }
        // services for adminpanel 
        //[BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
        //[RoutePrefix("api")]
        //public class CategoryController : ApiController
        //{
        [Route("GetAllCategoriesByStoreId")]
        public async Task<IHttpActionResult> GetAllCategoriesByStoreId(int StoreId)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    IEnumerable<Category> cat;
                    //var Store = ctx.Stores.FirstOrDefault(x => x.Id == StoreId);
                    //if (Store.BusinessType.Contains("Alcohol"))
                    //{
                    //    cat = ctx.Categories.Where(x => x.Store_Id == StoreId && x.ParentCategoryId !=0 && x.IsDeleted == false).OrderBy(x => x.Name).ToList();
                    //}
                    //else
                    //{
                        cat = ctx.Categories.Where(x => x.Store_Id == StoreId && x.IsDeleted == false).OrderBy(x => x.Name).ToList();

                    //}
                    CustomResponse<IEnumerable<Category>> response = new CustomResponse<IEnumerable<Category>>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = cat
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [Route("GetCategoriesByStoreId")]
        public async Task<IHttpActionResult> GetAllCategoriesByStoreIdUser(int StoreId)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    CustomResponse<Models.Admin.CategoriesViewModel> response = new CustomResponse<Models.Admin.CategoriesViewModel>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new Models.Admin.CategoriesViewModel { Categories = ctx.Categories.Where(x => x.Store_Id == StoreId && x.ParentCategoryId == 0 && x.IsDeleted == false).OrderBy(x => x.Name).ToList() }

                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [Route("GetSubCategoriesByCatId")]
        public async Task<IHttpActionResult> GetSubCategoriesByCatId(int CatId)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var categories = ctx.Categories.Where(x => x.ParentCategoryId == CatId && x.IsDeleted == false).OrderBy(x => x.Name).ToList();
                    categories.Insert(0, new Category { Name = "All", Id = CatId });
                    CustomResponse<Models.Admin.CategoriesViewModel> response = new CustomResponse<Models.Admin.CategoriesViewModel>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new Models.Admin.CategoriesViewModel { Categories = categories }
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


    }
}


