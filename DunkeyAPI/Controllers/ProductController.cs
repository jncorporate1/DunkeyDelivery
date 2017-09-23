using AutoMapper;
using DAL;
using DunkeyAPI.Models;
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
                return StatusCode(Utility.LogError(ex));
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
                return StatusCode(Utility.LogError(ex));
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
                model.Price = httpRequest.Params["Price"];
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
                            Image = Utility.BaseUrl + ConfigurationManager.AppSettings["ProductImageFolderPath"] + Path.GetFileName(newFullPath)
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
                return StatusCode(Utility.LogError(ex));
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
                        .Where(x => x.Category_Id == Category_Id).Include(x => x.Store)
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
                        .Where(x => x.Category_Id == Category_Id).ToList();
                   
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
                            query += " WHERE Products.Name Like '%"+search_string.Trim()+"%' ";

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

                    //var res = ctx.Products
                    //    .Where(x => x.Name.StartsWith(search_string)).ToList();
                    //ClientProductViewModel model = new ClientProductViewModel();
                    //model= Mapper.Map<ClientProductViewModel>(res);

                     CustomResponse<List<Product>> response = new CustomResponse<List<Product>>
                    //CustomResponse<IEnumerable<Product>> response = new CustomResponse<IEnumerable<Product>>
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
                    var response = ctx.Products.Where(x => x.Name.Contains(search_string) && x.Store_Id==Store_Id).ToList();
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
                        .Where(x => x.Category_Id == Category_Id).OrderBy(x=>x.Image).Skip(Page*Items).Take(Items).ToList();
                    var f = Mapper.Map<List<productslist>>(res);
                    responsee.productslist = f;

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
        [Route("ProductByName")]
        public IHttpActionResult ProductByName(string search_string, int Category_id = 10)
        {
            try
            {

                Stores Utility = new Stores();
                CategoryProductViewModel products = new CategoryProductViewModel();
                var Type = Utility.Categories_enum(Category_id);
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var query = "SELECT Products.* FROM Stores INNER JOIN Products ON Products.Store_id = Stores.Id";
                    if (Type == "All")
                    {

                        if (string.IsNullOrEmpty(search_string))
                        {

                        }
                        else
                        {
                            query += " WHERE Products.Name Like '%" + search_string.Trim() + "%' ";

                        }


                    }
                    else
                    {
                        query += " WHERE Stores.BusinessType='" + Type + "'";
                        if (string.IsNullOrEmpty(search_string))
                        {

                        }
                        else
                        {
                            query += " AND Products.Name Like '%" + search_string.Trim() + "%'";
                        }


                    }

                    products.productslist = ctx.Database.SqlQuery<productslist>(query).ToList();
                    
                    //var res = ctx.Products
                    //    .Where(x => x.Name.StartsWith(search_string)).ToList();
                    //ClientProductViewModel model = new ClientProductViewModel();
                    //model= Mapper.Map<ClientProductViewModel>(res);

                    CustomResponse<CategoryProductViewModel> response = new CustomResponse<CategoryProductViewModel>
                    //CustomResponse<IEnumerable<Product>> response = new CustomResponse<IEnumerable<Product>>
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
                    var productList = ctx.Products.Where(x=>x.Store_Id==Store_Id).ToList();
                    res.productslist = Mapper.Map<List<productslist>>(productList);
                    //res.TotalRecords = ctx.Products.Where(x => x.Store.BusinessType == CategoryType).Count();
                }
                else
                {
                    var productList = ctx.Products.Where(x => x.Name.Contains(search_string) && x.Store_Id== Store_Id).ToList();

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

    }
}
