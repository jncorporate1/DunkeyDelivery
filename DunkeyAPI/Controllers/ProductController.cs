using AutoMapper;
using DAL;
using DunkeyAPI.Models;
using DunkeyAPI.Models.Admin;
using DunkeyAPI.ViewModels;
using DunkeyDelivery;
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
using static DunkeyAPI.Utility.Global;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Products")]
    public class ProductController : ApiController
    {
        [Route("Create")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Create(ProductBindingModel model)
        {
            try
            {
                short Status = model.Status;
                if (Status != 1)
                {
                    model.Status = 0;
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (ctx.Products.Any(x => x.Category_Id == model.Category_Id && x.Name == model.Name))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "Product already exists under same store and category" }
                        });
                    }
                    else
                    {
                        var newProductModel = new Product { Category_Id = model.Category_Id, Name = model.Name, Description = model.Description, Price = model.Price, Status = 0, Store_Id = model.Store_Id };
                        ctx.Products.Add(newProductModel);
                        ctx.SaveChanges();
                        CustomResponse<Product> response = new CustomResponse<Product> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = newProductModel };
                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("AddProduct")]
        public async Task<IHttpActionResult> AddProduct(ProductBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var newProductModel = new Product { Store_Id = model.Store_Id, Category_Id = model.Category_Id, Name = model.Name, Description = model.Description, Price = model.Price, Status = 0 };
                    ctx.Products.Add(newProductModel);
                    ctx.SaveChanges();
                    CustomResponse<Product> response = new CustomResponse<Product> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = newProductModel };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [Route("AddProductWithImage")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> AddProductWithImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                ProductBindingModel model = new ProductBindingModel();
                model.Name = httpRequest.Params["Name"];
                model.Price =Convert.ToDouble(httpRequest.Params["Price"]);
                model.Category_Id = Convert.ToInt32(httpRequest.Params["CatId"]);
                model.Description = httpRequest.Params["Description"];
                model.Store_Id = Convert.ToInt32(httpRequest.Params["StoreId"]);

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
                    if (ctx.Products.Any(x => x.Category_Id == model.Category_Id && x.Name == model.Name))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "Product already exists under same store and category" }
                        });
                    }
                    else
                    {
                        #region ImageSaving
                        var postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
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
                            else if (postedFile.ContentLength > Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + Global.ImageSize + "." }
                                });
                            }
                            else
                            {
                                int count = 1;
                                fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + postedFile.FileName);

                                while (File.Exists(newFullPath))
                                {
                                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + tempFileName + extension);
                                }
                                postedFile.SaveAs(newFullPath);
                            }
                        }
                        #endregion

                        //var RequestUriString = Request.RequestUri.ToString();

                        //var baseUrl = RequestUriString.Substring(0, RequestUriString.IndexOf("api"));

                        Product productModel = new Product
                        {
                            Name = model.Name,
                            Category_Id = model.Category_Id,
                            Description = model.Description,
                            Price = model.Price,
                            Store_Id = model.Store_Id,
                            Image = DunkeyDelivery.Utility.BaseUrl + ConfigurationManager.AppSettings["ProductImageFolderPath"] + Path.GetFileName(newFullPath)
                        };

                        ctx.Products.Add(productModel);
                        ctx.SaveChanges();

                        CustomResponse<Product> response = new CustomResponse<Product>
                        {
                            Message = Global.SuccessMessage,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = productModel
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

        //[HttpGet]
        //[Route("GetCategoryProducts")]
        //public IHttpActionResult GetCategoryProducts(short Category_Id)
        //{
        //    try
        //    {
        //        DunkeyContext ctx = new DunkeyContext();
        //        var res = ctx.Products.Where(x => x.Category_Id == Category_Id).ToList();

        //        if (res.Count() == 0)
        //        {
        //            CustomResponse<IEnumerable<Product>> response = new CustomResponse<IEnumerable<Product>>
        //            {
        //                Message = "Success",
        //                StatusCode = (int)HttpStatusCode.OK,
        //                Result = res
        //            };
        //            return Ok(response);

        //        }
        //        else
        //        {
        //            CustomResponse<IEnumerable<Product>> response = new CustomResponse<IEnumerable<Product>>
        //            { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = res };
        //            return Ok(response);


        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(DunkeyDelivery.Utility.LogError(ex));
        //    }
        //    return Ok();
        //}
        
        [HttpGet]
        [Route("ProductsByCategory")]
        public IHttpActionResult SearchByCategory(short Category_Id)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var res = ctx.Products
                        .Where(x => x.Category_Id == Category_Id && x.IsDeleted==false).Include(x => x.Store)
                        .ToList();
                    CustomResponse<IEnumerable<Product>> response = new CustomResponse<IEnumerable<Product>>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = res
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        // Services For Mobile End Interface 
        
        [HttpGet]
        [Route("GetProductsByCategory")]
        public IHttpActionResult GetProductsByCategory(short Category_Id)
        {
            try
            {
               

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var res = ctx.Products
                        .Where(x => x.Category_Id == Category_Id && x.IsDeleted==false).ToList();
                   
                    CustomResponse<IEnumerable<Product>> response = new CustomResponse<IEnumerable<Product>>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = res
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
        [Route("SearchProductByName")]
        public IHttpActionResult SearchProductByName(string search_string,int Category_id=10)   
        {
            try
            {
               
                Stores Utility = new Stores();
                List<Product> products = new List<Product>();
                var Type=Utility.Categories_enum(Category_id);
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var query = "SELECT Products.* FROM Stores INNER JOIN Products ON Products.Store_id = Stores.Id";
                    if (Type=="All")
                    {
                        
                        if (string.IsNullOrEmpty(search_string))
                        {
                            
                        }
                        else
                        {
                            query += " WHERE Products.IsDeleted=0 AND Products.Name Like '%"+search_string.Trim()+"%' ";

                        }


                    }else
                    {
                        query += " WHERE Stores.BusinessType='" + Type + "'";
                        if (string.IsNullOrEmpty(search_string))
                        {
                           
                        }
                        else
                        {
                            query += " AND Products.Name Like '%"+search_string.Trim()+"%'";
                        }


                    }

                    products = ctx.Database.SqlQuery<Product>(query).ToList();


                    foreach (var prod in products)
                    {
                        var Store = ctx.Stores.FirstOrDefault(x => x.Id == prod.Store_Id);
                        prod.BusinessName = Store.BusinessName;
                        prod.BusinessType = Store.BusinessType;
                        prod.MinDeliveryCharges = Store.MinDeliveryCharges;
                        prod.MinDeliveryTime = Store.MinDeliveryTime;
                        prod.MinOrderPrice = Store.MinOrderPrice;
                    }


                    CustomResponse<List<Product>> response = new CustomResponse<List<Product>>
                    {
                        Message = "Success",
                       StatusCode = (int)HttpStatusCode.OK,
                        Result = products
                     };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        // mobile services 
        [HttpGet]
        [Route("GetMedicationNames")]
        public IHttpActionResult GetMedicationNames(short Store_Id,string search_string)
        {
            try
            {
                using (DunkeyContext ctx=new DunkeyContext())
                {
                    var response = ctx.Products.Where(x => x.Name.Contains(search_string) && x.Store_Id==Store_Id && x.IsDeleted==false).ToList();
                    var f = Mapper.Map<List<MedicationNames>>(response);
                    Medications responseModel = new Medications { medications = f };


                    CustomResponse<Medications> res = new CustomResponse<Medications>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = responseModel
                    };
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        [HttpGet]
        [Route("GetCategoryProducts")]
        public IHttpActionResult GetCategoryProducts(short Category_Id,int Page=0,int Items=20)
        {
            try
            {

                CategoryProductViewModel responsee = new CategoryProductViewModel();
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var res = ctx.Products
                        .Where(x => x.Category_Id == Category_Id && x.IsDeleted==false).OrderBy(x=>x.Image).Skip(Page*Items).Take(Items).ToList();
                    var f = Mapper.Map<List<productslist>>(res);
                    responsee.productslist = f;

                    foreach (var item in responsee.productslist)
                    {
                        var store = ctx.Stores.FirstOrDefault(x => x.Id == item.Store_id);
                        item.BusinessName = store.BusinessName;
                        item.BusinessType = store.BusinessType;
                        item.MinDeliveryCharges = store.MinDeliveryCharges;
                        item.MinDeliveryTime = store.MinDeliveryTime;
                        item.MinOrderPrice = store.MinOrderPrice;
                    }

                    responsee.TotalRecords = ctx.Products.Where(x => x.Category_Id == Category_Id).Count();
                    CustomResponse<CategoryProductViewModel> response = new CustomResponse<CategoryProductViewModel>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = responsee
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
        [Route("GetStoreProducts")]
        public IHttpActionResult GetStoreProducts(short Store_id, int Page = 0, int Items = 10)
        {
            try
            {

                CategoryProductViewModel responsee = new CategoryProductViewModel();
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var res = ctx.Products
                        .Where(x => x.Store_Id == Store_id && x.IsDeleted == false).OrderBy(x => x.Name).Skip(Page * Items).Take(Items).ToList();
                    var f = Mapper.Map<List<productslist>>(res);
                    responsee.productslist = f;

                    responsee.TotalRecords = ctx.Products.Where(x => x.Store_Id == Store_id && x.IsDeleted == false).Count();
                    CustomResponse<CategoryProductViewModel> response = new CustomResponse<CategoryProductViewModel>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = responsee
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
        [Route("ProductByName")]
        public IHttpActionResult ProductByName(string search_string, int Category_id=0 ,int Category_Type=10,double? latitude=0,double? longitude=0,int Store_id=0,int Items=6,int Page=0)
        {
            try
            {

                Stores Utility = new Stores();
                CPVM products = new CPVM();
                var Type = Utility.Categories_enum(Category_Type);
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var query = "";
                    var ExtendedQuery = "Where Products.IsDeleted=0 AND ";
                    if (latitude != 0 && longitude != 0)
                    {

                        query = "SELECT Products.*,Stores.BusinessName ,Stores.Location.STDistance('POINT(" + longitude + " " + latitude + ")') as Distance FROM Stores INNER JOIN Products ON Products.Store_id = Stores.Id ";
                        ExtendedQuery = ExtendedQuery + " Stores.Location.STDistance('POINT("+ longitude + " "+latitude+ ")') <= 80467.2 ";

                    }
                    else
                    {
                        query = "SELECT Products.*,Stores.BusinessName FROM Stores INNER JOIN Products ON Products.Store_id = Stores.Id  ";
                    }
                    if (Store_id == 0)
                    { 

                            if (Category_id == 0)// if there is no category id
                            {
                            if (Type == "All")
                            {

                                if (string.IsNullOrEmpty(search_string))
                                {

                                }
                                else
                                {
                                    if (latitude != 0 && longitude != 0)
                                    {
                                        ExtendedQuery = ExtendedQuery + " AND ";
                                    }
                                    query += " "+ ExtendedQuery + " Products.Name Like '%" + search_string.Trim() + "%' ";

                                }


                            }
                            else
                            {
                                if (latitude != 0 && longitude != 0)
                                {
                                    ExtendedQuery = ExtendedQuery + " AND ";
                                }
                                query += " " + ExtendedQuery + " Stores.BusinessType='" + Type + "'";
                                if (string.IsNullOrEmpty(search_string))
                                {

                                }
                                else
                                {
                                    query += " AND Products.Name Like '%" + search_string.Trim() + "%'";
                                }


                            }

                        }
                        else
                        {

                            if (Type == "All")
                            {

                                if (string.IsNullOrEmpty(search_string))
                                {

                                }
                                else
                                {
                                    if (latitude != 0 && longitude != 0)
                                    {
                                        ExtendedQuery = ExtendedQuery + " AND ";
                                    }
                                    query += " " + ExtendedQuery + " Products.Name Like '%" + search_string.Trim() + "%'  AND Products.Category_Id=" + Category_id + " ";

                                }


                            }
                            else
                            {
                                if (latitude != 0 && longitude != 0)
                                {
                                    ExtendedQuery = ExtendedQuery + " AND ";
                                }
                                query += " " + ExtendedQuery + " Stores.BusinessType='" + Type + "' AND Products.Category_Id=" + Category_id + " ";
                                if (string.IsNullOrEmpty(search_string))
                                {

                                }
                                else
                                {
                                    query += " AND Products.Name Like '%" + search_string.Trim() + "%'  ";
                                }


                            }
                        }
                    }else
                    {
                        if(latitude !=0 && longitude != 0)
                        {
                            ExtendedQuery = ExtendedQuery + " AND ";
                        }
                        query += " " + ExtendedQuery + " Products.Store_Id=" + Store_id+ " AND Products.Name Like '%" + search_string.Trim() + "%'  ";

                    }

                    if(Category_Type != 4)
                    {
                        query += "AND Stores.BusinessType != 'Pharmacy'  ORDER BY Products.Name OFFSET " + Items * Page + " ROWS FETCH NEXT " + Items + " ROWS ONLY";

                    }else
                    {
                        query += " ORDER BY Products.Name OFFSET " + Items * Page + " ROWS FETCH NEXT " + Items + " ROWS ONLY";

                    }
                    products.productslist = ctx.Database.SqlQuery<PL>(query).ToList();

                    foreach (var prod in products.productslist)
                    {
                        var Store = ctx.Stores.FirstOrDefault(x => x.Id == prod.Store_id);
                        prod.BusinessName = Store.BusinessName;
                        prod.BusinessType= Store.BusinessType;
                        prod.MinDeliveryCharges = Store.MinDeliveryCharges;
                        prod.MinDeliveryTime = Store.MinDeliveryTime;
                        prod.MinOrderPrice = Store.MinOrderPrice;
                    }


                    CustomResponse<CPVM> response = new CustomResponse<CPVM>

                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = products
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
        [Route("GetAlcoholStores")]
        public IHttpActionResult GetAlcoholStores(string search_string, string CategoryType,int Store_Id)
        {
            try
            {
                CategoryProductViewModel res = new CategoryProductViewModel();
                DunkeyContext ctx = new DunkeyContext();
              
                if (string.IsNullOrEmpty(search_string))
                {
                    var productList = ctx.Products.Where(x=>x.Store_Id== Store_Id && x.IsDeleted == false).ToList();
                    res.productslist = Mapper.Map<List<productslist>>(productList);
                    //res.TotalRecords = ctx.Products.Where(x => x.Store.BusinessType == CategoryType).Count();
                }
                else
                {
                    var productList = ctx.Products.Where(x => x.Name.Contains(search_string) && x.Store_Id== Store_Id && x.IsDeleted == false).ToList();

                    res.productslist = Mapper.Map<List<productslist>>(productList);
                }
        
                CustomResponse<CategoryProductViewModel> response = new CustomResponse<CategoryProductViewModel>
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
        // admin panel services 
        
        [HttpGet]
        [Route("GetProductsByCategoryId")]
        public async Task<IHttpActionResult> GetProductsByCategoryId(int CatId, int UserId, int PageSize, int PageNo, string filterTypes = "", bool IsAll = false)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var userFavourites = ctx.Favourites.Where(x => x.User_ID == UserId && x.IsDeleted == false).ToList();
                    List<Product> products;
                    int TotalCount;

                    if (IsAll)
                    {
                        var CatIds = ctx.Categories.Where(x => x.Id == CatId || x.ParentCategoryId == CatId && x.IsDeleted == false).Select(x => x.Id).ToList();
                        TotalCount = ctx.Products.Count(x => CatIds.Contains(x.Category_Id.Value) && x.IsDeleted == false);
                        products = ctx.Products.Where(x => CatIds.Contains(x.Category_Id.Value) && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }
                    else
                    {
                        TotalCount = ctx.Products.Count(x => x.Category_Id == CatId && x.IsDeleted == false);
                        products = ctx.Products.Where(x => x.Category_Id == CatId && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }

                    //foreach (var product in products)
                    //{
                    //    product.Weight = Convert.ToString(product.WeightInGrams) + " gm";

                    //    if (userFavourites.Any(x => x.Product_Id == product.Id))
                    //        product.IsFavourite = true;
                    //    else
                    //        product.IsFavourite = false;
                    //}

                    return Ok(new CustomResponse<ProductsViewModel>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new ProductsViewModel
                        {
                            Count = TotalCount,
                            Products = products
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetProductCount")]
        public async Task<IHttpActionResult> GetProductCount()
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    ProductCountViewModel model = new ProductCountViewModel { TotalProducts = ctx.Products.Count(x => x.IsDeleted == false) };
                    CustomResponse<ProductCountViewModel> response = new CustomResponse<ProductCountViewModel>
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

        [HttpGet]
        [Route("GetAllProductsByStoreId")]
        public async Task<IHttpActionResult> GetAllProductsByStoreId(int StoreId, int UserId, int PageSize, int PageNo, string filterTypes = "")
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    return Ok(new CustomResponse<ProductsViewModel>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new ProductsViewModel
                        {
                            Count = ctx.Products.Count(x => x.Store_Id == StoreId && x.IsDeleted == false),
                            Products = ctx.Products.Where(x => x.Store_Id == StoreId && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

    }
}
