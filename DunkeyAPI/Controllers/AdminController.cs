﻿using DAL;
using DunkeyAPI.BindingModels;
using DunkeyAPI.Models.Admin;
using DunkeyAPI.Utility;
using DunkeyAPI.ViewModels;
using DunkeyDelivery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static DunkeyAPI.Utility.Global;
using static DunkeyDelivery.Utility;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        //[HttpGet]
        //[Route("GetDashboardStats")]
        //public IHttpActionResult GetDashboardStats()
        //{
        //    try
        //    {
        //        DunkeyContext ctx = new DunkeyContext();
        //        WebDashboardStatsViewModel model = new WebDashboardStatsViewModel
        //        {
        //            TotalUsers=ctx.Users.Count(),
        //            TotalStores=ctx.Stores.Count(),
        //            TotalProducts = ctx.Stores.Count()


        //        };

        //        CustomResponse<WebDashboardStatsViewModel> response = new CustomResponse<WebDashboardStatsViewModel>
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

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Get Dashboard Stats
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[Route("GetAdminDashboardStats")]
        //public async Task<IHttpActionResult> GetAdminDashboardStats()
        //{
        //    try
        //    {
        //        using (DunkeyContext ctx = new DunkeyContext())
        //        {
        //            DateTime TodayDate = DateTime.Now.Date;
        //            WebDashboardStatsViewModel model = new WebDashboardStatsViewModel { TotalProducts = ctx.Products.Count(x => x.IsDeleted == false), TotalStores = ctx.Stores.Count(), TotalUsers = ctx.Users.Count(), TodayOrders = ctx.Orders.Count(x => DbFunctions.TruncateTime(x.OrderDateTime) == TodayDate.Date) };
        //            CustomResponse<WebDashboardStatsViewModel> response = new CustomResponse<WebDashboardStatsViewModel>
        //            {
        //                Message = ResponseMessages.Success,
        //                StatusCode = (int)HttpStatusCode.OK,
        //                Result = model
        //            };
        //            return Ok(response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(DunkeyDelivery.Utility.LogError(ex));
        //    }
        //}

        // admin panel services 

       // [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Add admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAdmin")]
        public async Task<IHttpActionResult> AddAdmin()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                DAL.Admin model = new DAL.Admin();
                DAL.Admin existingAdmin = new DAL.Admin();

                if (httpRequest.Params["Id"] != null)
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);

                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);

                model.FirstName = httpRequest.Params["FirstName"];
                model.LastName = httpRequest.Params["LastName"];
                model.Email = httpRequest.Params["Email"];
                model.Phone = httpRequest.Params["Phone"];
                model.Role = Convert.ToInt16(httpRequest.Params["Role"]);
                model.Password = httpRequest.Params["Password"];
                model.Status = (int)DunkeyDelivery.Global.StatusCode.NotVerified;

                if (httpRequest.Params["Store_Id"] != null)
                    model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);

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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (model.Id == 0)
                    {

                        if (ctx.Admins.Any(x => x.Email == model.Email && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Admin with same email already exists" }
                            });
                        }
                    }
                    else
                    {
                        existingAdmin = ctx.Admins.FirstOrDefault(x => x.Id == model.Id);
                        model.Password = existingAdmin.Password;
                        if (existingAdmin.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) == false || existingAdmin.Store_Id != model.Store_Id)
                        {
                            if (ctx.Admins.Any(x => x.IsDeleted == false && x.Store_Id == model.Store_Id && x.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Admin with same email already exists" }
                                });
                            }
                        }
                    }

                    string fileExtension = string.Empty;
                    HttpPostedFile postedFile = null;
                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = Path.GetExtension(postedFile.FileName);
                            fileExtension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(fileExtension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > DunkeyDelivery.Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + DunkeyDelivery.Global.ImageSize }
                                });
                            }
                            else
                            {
                                //int count = 1;
                                //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + postedFile.FileName);

                                //while (File.Exists(newFullPath))
                                //{
                                //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + tempFileName + extension);
                                //}
                                //postedFile.SaveAs(newFullPath);
                            }
                        }
                        //model.ImageUrl = ConfigurationManager.AppSettings["AdminImageFolderPath"] + Path.GetFileName(newFullPath);
                    }
                    #endregion

                    if (model.Id == 0)
                    {
                        ctx.Admins.Add(model);
                        ctx.SaveChanges();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + model.Id + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        model.ImageUrl = ConfigurationManager.AppSettings["AdminImageFolderPath"] + model.Id + fileExtension;
                        ctx.SaveChanges();

                    }
                    else
                    {
                        //existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (model.ImageDeletedOnEdit == false)
                            {
                                model.ImageUrl = existingAdmin.ImageUrl;
                            }
                        }
                        else
                        {
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + model.Id + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            model.ImageUrl = ConfigurationManager.AppSettings["AdminImageFolderPath"] + model.Id + fileExtension;
                        }

                        ctx.Entry(existingAdmin).CurrentValues.SetValues(model);
                        ctx.SaveChanges();
                    }

                    await model.GenerateToken(Request);

                    CustomResponse<DAL.Admin> response = new CustomResponse<DAL.Admin>
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

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Add category with image. This is multipart request
        /// </summary>
        /// <returns></returns>
        [Route("AddCategory")]
        public async Task<IHttpActionResult> AddCategoryWithImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                Category model = new Category();
                Category existingCategory = new Category();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }
                if (httpRequest.Params["ParentCategoryId"] != null)
                {
                    model.ParentCategoryId = Convert.ToInt32(httpRequest.Params["ParentCategoryId"]);
                }
                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                {
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                }
                model.Name = httpRequest.Params["Name"];
                model.Description = httpRequest.Params["Description"];
                model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);

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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Categories.Any(x => x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase) && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Category already exist under same store" }
                            });
                        }
                    }
                    else
                    {
                        existingCategory = ctx.Categories.FirstOrDefault(x => x.Id == model.Id);
                        if (existingCategory.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false || existingCategory.Store_Id != model.Store_Id)
                        {
                            if (ctx.Categories.Any(x => x.IsDeleted == false && x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Category with same name already exist under same store" }
                                });
                            }
                        }

                        if (existingCategory.Id == model.ParentCategoryId)
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Parent category name and child category name must be different" }
                            });
                        }
                    }

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0 && Request.Content.IsMimeMultipartContent())
                        {
                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = Path.GetExtension(postedFile.FileName);
                            fileExtension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(fileExtension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > DunkeyDelivery.Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " +  DunkeyDelivery.Global.ImageSize }
                                });
                            }
                            else
                            {
                                //int count = 1;
                                //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["CategoryImageFolderPath"] + postedFile.FileName);

                                //while (File.Exists(newFullPath))
                                //{
                                //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["CategoryImageFolderPath"] + tempFileName + extension);
                                //}
                                //postedFile.SaveAs(newFullPath);
                            }
                        }
                        //model.ImageUrl = ConfigurationManager.AppSettings["CategoryImageFolderPath"] + Path.GetFileName(newFullPath);
                    }
                    #endregion


                    if (model.Id == 0)
                    {
                        ctx.Categories.Add(model);
                        ctx.SaveChanges();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["CategoryImageFolderPath"] + model.Id + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        model.ImageUrl = ConfigurationManager.AppSettings["CategoryImageFolderPath"] + model.Id + fileExtension;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //var existingCategory = ctx.Categories.FirstOrDefault(x => x.Id == model.Id);
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (model.ImageDeletedOnEdit == false)
                            {
                                model.ImageUrl = existingCategory.ImageUrl;
                            }
                        }
                        else
                        {
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["CategoryImageFolderPath"] + model.Id + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            model.ImageUrl = ConfigurationManager.AppSettings["CategoryImageFolderPath"] + model.Id + fileExtension;
                        }

                        ctx.Entry(existingCategory).CurrentValues.SetValues(model);
                        ctx.SaveChanges();
                    }


                    CustomResponse<Category> response = new CustomResponse<Category>
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

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Add product with image. This is multipart request
        /// </summary>
        /// <returns></returns>
        [Route("AddProduct")]
        public async Task<IHttpActionResult> AddProductWithImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                Product model = new Product();
                Product existingProduct = new Product();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }

                //if (httpRequest.Params["Weight"] != null)
                //{
                //    model.WeightInGrams = Convert.ToDouble(httpRequest.Params["Weight"]);
                //}
                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                {
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                }
                model.Name = httpRequest.Params["Name"];
                model.Price = Convert.ToDouble(httpRequest.Params["Price"]);
                model.Category_Id = Convert.ToInt32(httpRequest.Params["Category_Id"]);
                model.Description = httpRequest.Params["Description"];
                model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);
                if (!string.IsNullOrEmpty(httpRequest.Params["Size"]))
                {
                    model.Size = httpRequest.Params["Size"];
                }
                


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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Products.Any(x => x.Category_Id == model.Category_Id && x.Name == model.Name && x.Store_Id == model.Store_Id && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Product already exist under same store and category" }
                            });
                        }
                    }
                    else
                    {
                        existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
                        if (existingProduct.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false || existingProduct.Category_Id != model.Category_Id || existingProduct.Store_Id != model.Store_Id)
                        {
                            if (ctx.Products.Any(x => x.IsDeleted == false && x.Category_Id == model.Category_Id && x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Product with same name already exist under same store and category" }
                                });
                            }
                        }
                    }

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = Path.GetExtension(postedFile.FileName);
                            fileExtension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(fileExtension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > DunkeyDelivery.Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + DunkeyDelivery.Global.ImageSize }
                                });
                            }
                            else
                            {
                                //int count = 1;
                                //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + postedFile.FileName);

                                //while (File.Exists(newFullPath))
                                //{
                                //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + tempFileName + fileExtension);
                                //}
                                //postedFile.SaveAs(newFullPath);
                            }
                        }
                        //model.ImageUrl = ConfigurationManager.AppSettings["ProductImageFolderPath"] + Path.GetFileName(newFullPath);
                    }
                    #endregion

                    if (model.Id == 0)
                    {
                        ctx.Products.Add(model);
                        ctx.SaveChanges();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        model.Image = ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + fileExtension;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (model.ImageDeletedOnEdit == false)
                            {
                                model.Image = existingProduct.Image;
                            }
                        }
                        else
                        {
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            model.Image = ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + fileExtension;
                        }

                        ctx.Entry(existingProduct).CurrentValues.SetValues(model);
                        ctx.SaveChanges();
                    }



                    CustomResponse<Product> response = new CustomResponse<Product>
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

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Add store with image, multipart request
        /// </summary>
        /// <returns></returns>
        [Route("AddStore")]
        public async Task<IHttpActionResult> AddStoreWithImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                StoreBindingModel model = new StoreBindingModel();
                Store existingStore = new Store();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }

                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                {
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                }

                model.BusinessName = httpRequest.Params["StoreName"];
                model.Latitude = Convert.ToDouble(httpRequest.Params["Lat"]);
                model.Longitude = Convert.ToDouble(httpRequest.Params["Long"]);
                model.Description = httpRequest.Params["Description"];
                model.Address = httpRequest.Params["Address"];
                model.BusinessType = httpRequest.Params["StoreType"];
                model.MinDeliveryCharges = 5;
                model.MinDeliveryTime = 45;
                model.MinOrderPrice = 15;

                TimeSpan openFrom, openTo;
                TimeSpan.TryParse(httpRequest.Params["Open_From"], out openFrom);
                TimeSpan.TryParse(httpRequest.Params["Open_To"], out openTo);

                if (openFrom != null)
                    model.Open_From = openFrom;

                if (openTo != null)
                    model.Open_To = openTo;

                if (httpRequest.Params["StoreDeliveryHours"] != null)
                {
                    var storeDeliveryHours = JsonConvert.DeserializeObject<StoreDeliveryHours>(httpRequest.Params["StoreDeliveryHours"]);

                    if (model.Id > 0)
                    {
                    }
                    else
                    {
                    }
                    model.StoreDeliveryHours = storeDeliveryHours;
                }


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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Stores.Any(x => x.BusinessName == model.BusinessName && x.Longitude == model.Longitude && x.Latitude == model.Latitude && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Store with same name and location already exists." }
                            });
                        }
                    }
                    else
                    {
                        existingStore = ctx.Stores.Include(x => x.StoreDeliveryHours).FirstOrDefault(x => x.Id == model.Id);
                        if (existingStore.BusinessName.Equals(model.BusinessName, StringComparison.InvariantCultureIgnoreCase) == false)
                        {
                            if (ctx.Stores.Any(x => x.IsDeleted == false && x.Id == model.Id && x.BusinessName.Equals(model.BusinessName.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Store with same name already exist" }
                                });
                            }
                        }
                    }

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB  

                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = Path.GetExtension(postedFile.FileName);
                            fileExtension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(fileExtension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > MaxContentLength)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto 1 mb" }
                                });
                            }
                            else
                            {
                                //    int count = 1;
                                //    fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + postedFile.FileName);

                                //    while (File.Exists(newFullPath))
                                //    {
                                //        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + tempFileName + fileExtension);
                                //    }
                                //    postedFile.SaveAs(newFullPath);
                            }
                        }
                        //model.ImageUrl = ConfigurationManager.AppSettings["StoreImageFolderPath"] + Path.GetFileName(newFullPath);
                    }
                    #endregion

                    Store storeModel = new Store();
                    storeModel.Id = model.Id;
                    storeModel.BusinessName = model.BusinessName;
                    storeModel.Open_From = model.Open_From;
                    storeModel.Open_To = model.Open_To;
                    storeModel.Description = model.Description;
                    storeModel.Latitude = model.Latitude;
                    storeModel.Longitude = model.Longitude;
                    storeModel.ImageUrl = model.ImageUrl;
                    storeModel.StoreDeliveryHours = model.StoreDeliveryHours;
                    storeModel.ImageDeletedOnEdit = model.ImageDeletedOnEdit;
                    storeModel.Location = DunkeyDelivery.Utility.CreatePoint(model.Latitude, model.Longitude);
                    storeModel.StoreDeliveryHours.Id = storeModel.Id;
                    storeModel.Address = model.Address;
                    storeModel.BusinessType = model.BusinessType;
                    storeModel.MinDeliveryCharges = 5;
                    storeModel.MinDeliveryTime = 45;
                    storeModel.MinOrderPrice = 15;

                    if (storeModel.Id == 0)
                    {
                        ctx.Stores.Add(storeModel);
                        ctx.SaveChanges();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + storeModel.Id + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        storeModel.ImageUrl = ConfigurationManager.AppSettings["StoreImageFolderPath"] + storeModel.Id + fileExtension;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (storeModel.ImageDeletedOnEdit == false)
                            {
                                storeModel.ImageUrl = existingStore.ImageUrl;
                            }
                        }
                        else
                        {
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + storeModel.Id + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            storeModel.ImageUrl = ConfigurationManager.AppSettings["StoreImageFolderPath"] + storeModel.Id + fileExtension;
                        }

                        ctx.Entry(existingStore).CurrentValues.SetValues(storeModel);

                        if (existingStore.StoreDeliveryHours == null)
                            ctx.StoreDeliveryHours.Add(storeModel.StoreDeliveryHours);
                        else
                            ctx.Entry(existingStore.StoreDeliveryHours).CurrentValues.SetValues(storeModel.StoreDeliveryHours);
                        ctx.SaveChanges();
                    }


                    CustomResponse<Store> response = new CustomResponse<Store>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = storeModel
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
        /// Get Dashboard Stats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAdminDashboardStats")]
        public async Task<IHttpActionResult> GetAdminDashboardStats()
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    DateTime TodayDate = DateTime.Now.Date;
                    WebDashboardStatsViewModel model = new WebDashboardStatsViewModel { TotalProducts = ctx.Products.Count(x => x.IsDeleted == false), TotalStores = ctx.Stores.Count(), TotalUsers = ctx.Users.Count(), TodayOrders = ctx.Orders.Count(x => DbFunctions.TruncateTime(x.OrderDateTime) == TodayDate.Date) };
                    CustomResponse<WebDashboardStatsViewModel> response = new CustomResponse<WebDashboardStatsViewModel>
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

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("SearchAdmins")]
        public async Task<IHttpActionResult> SearchAdmins(string FirstName, string LastName, string Email, string Phone, int? StoreId)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    string conditions = string.Empty;

                    if (!String.IsNullOrEmpty(FirstName))
                        conditions += " And Admins.FirstName Like '%" + FirstName.Trim() + "%'";

                    if (!String.IsNullOrEmpty(LastName))
                        conditions += " And Admins.LastName Like '%" + LastName.Trim() + "%'";

                    if (!String.IsNullOrEmpty(Email))
                        conditions += " And Admins.Email Like '%" + Email.Trim() + "%'";

                    if (!String.IsNullOrEmpty(Phone))
                        conditions += " And Admins.Phone Like '%" + Phone.Trim() + "%'";

                    if (StoreId.HasValue && StoreId.Value != 0)
                        conditions += " And Admins.Store_Id = " + StoreId;

                    #region query
                    var query = @"SELECT
  Admins.Id,
  Admins.FirstName,
  Admins.LastName,
  Admins.Email,
  Admins.Phone,
  Admins.Role,
  Admins.ImageUrl,
  Stores.BusinessName AS StoreName
FROM Admins
LEFT OUTER JOIN Stores
  ON Stores.Id = Admins.Store_Id
WHERE Admins.IsDeleted = 0
AND Stores.IsDeleted = 0 " + conditions + @" UNION
SELECT
  Admins.Id,
  Admins.FirstName,
  Admins.LastName,
  Admins.Email,
  Admins.Phone,
  Admins.Role,
  Admins.ImageUrl,
  '' AS StoreName
FROM Admins
WHERE Admins.IsDeleted = 0
AND ISNULL(Admins.Store_Id, 0) = 0 " + conditions;

                    #endregion


                    var admins = ctx.Database.SqlQuery<SearchAdminViewModel>(query).ToList();

                    return Ok(new CustomResponse<SearchAdminListViewModel> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchAdminListViewModel { Admins = admins } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
        [HttpGet]
        [Route("SearchProducts")]
        public async Task<IHttpActionResult> SearchProducts(string ProductName, float? ProductPrice, string CategoryName, int? StoreId)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var query = "select Products.*, Stores.BusinessName as StoreName, Categories.Name as CategoryName from Products join Categories on Products.Category_Id = Categories.Id join Stores on Products.Store_Id = Stores.Id where Products.IsDeleted = 0 and Categories.IsDeleted = 0 and Stores.IsDeleted = 0";
                    if (!String.IsNullOrEmpty(CategoryName))
                        query += " And Categories.Name Like '%" + CategoryName + "%'";

                    if (!String.IsNullOrEmpty(ProductName))
                        query += " And Products.Name Like '%" + ProductName + "%'";

                    if (ProductPrice.HasValue)
                        query += " And Price = " + ProductPrice.Value;

                    if (StoreId.HasValue && StoreId.Value != 0)
                        query += " And Products.Store_Id = " + StoreId;

                    var products = ctx.Database.SqlQuery<SearchProductViewModel>(query).ToList();

                    foreach (var product in products)
                    {
                        product.Weight = Convert.ToString(product.WeightInGrams) + " gm";
                    }
                    return Ok(new CustomResponse<SearchProductListViewModel> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchProductListViewModel { Products = products } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
        [HttpGet]
        [Route("SearchCategories")]
        public async Task<IHttpActionResult> SearchCategories(string CategoryName, int? StoreId)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var query = "select Categories.*, Stores.BusinessName as StoreName from Categories join Stores on Categories.Store_Id = Stores.Id where Categories.IsDeleted = 0 and Stores.IsDeleted = 0";

                    if (!String.IsNullOrEmpty(CategoryName))
                        query += " And Categories.Name Like '%" + CategoryName + "%'";

                    if (StoreId.HasValue && StoreId.Value != 0)
                        query += " And Categories.Store_Id = " + StoreId;

                    var categories = ctx.Database.SqlQuery<SearchCategoryViewModel>(query).ToList();
                    return Ok(new CustomResponse<SearchCategoryListViewModel> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchCategoryListViewModel { Categories = categories } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
        [HttpGet]
        [Route("SearchOffers")]
        public async Task<IHttpActionResult> SearchOffers(string OfferName, int? StoreId)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var query = "select Offers.*, Stores.BusinessName as StoreName from Offers join Stores on Offers.Store_Id = Stores.Id where Offers.IsDeleted = 0 and Stores.IsDeleted = 0";

                    if (!String.IsNullOrEmpty(OfferName))
                        query += " And Offers.Name Like '%" + OfferName + "%'";

                    if (StoreId.HasValue && StoreId.Value != 0)
                        query += " And Offers.Store_Id = " + StoreId;

                    var offers = ctx.Database.SqlQuery<SearchOfferViewModel>(query).ToList();
                    return Ok(new CustomResponse<SearchOfferListViewModel> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchOfferListViewModel { Offers = offers } });
                    //return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
        [HttpGet]
        [Route("SearchPackages")]
        public async Task<IHttpActionResult> SearchPackages(string PackageName, int? StoreId)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var query = "select Packages.*, Stores.BusinessName as StoreName from Packages join Stores on Packages.Store_Id = Stores.Id where Packages.IsDeleted = 0 and Stores.IsDeleted = 0";

                    if (!String.IsNullOrEmpty(PackageName))
                        query += " And Packages.Name Like '%" + PackageName + "%'";

                    if (StoreId.HasValue && StoreId.Value != 0)
                        query += " And Packages.Store_Id = " + StoreId;

                    var packages = ctx.Database.SqlQuery<SearchPackageViewModel>(query).ToList();
                    return Ok(new CustomResponse<SearchPackageListViewModel> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchPackageListViewModel { Packages = packages } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("DeleteEntity")]
        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        public async Task<IHttpActionResult> DeleteEntity(int EntityType, int Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    switch (EntityType)
                    {
                        case (int)DunkeyEntityTypes.Product:
                            ctx.Products.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            break;
                        case (int)DunkeyEntityTypes.Category:
                            ctx.Categories.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            ctx.Database.ExecuteSqlCommand("update products set isdeleted = 1 where category_id = " + Id);
                            break;
                        case (int)DunkeyEntityTypes.Store:
                            ctx.Stores.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            ctx.Database.ExecuteSqlCommand("update products set isdeleted = 1 where store_id = " + Id + @"; 
                            update packages set isdeleted = 1 where store_id = " + Id + @"; 
                            update categories set isdeleted = 1 where store_id = " + Id + @"; 
                            update offers set isdeleted = 1 where store_id = " + Id);
                            break;
                        case (int)DunkeyEntityTypes.Package:
                            ctx.Packages.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            break;
                        case (int)DunkeyEntityTypes.Admin:
                            ctx.Admins.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            break;
                        case (int)DunkeyEntityTypes.Offer:
                            ctx.Offers.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            break;
                        default:
                            break;
                    }
                    ctx.SaveChanges();
                    return Ok(new CustomResponse<string> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

//        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
//        [HttpGet]
//        [Route("GetReadyForDeliveryOrders")]
//        public async Task<IHttpActionResult> GetReadyForDeliveryOrders()
//        {
//            try
//            {
//                using (DunkeyContext ctx = new DunkeyContext())
//                {
//                    #region query
//                    var query = @"
//select 
//Orders.Id,
//Orders.OrderDateTime as CreatedOn,
//Orders.Total as OrderTotal,
//Orders.DeliveryMan_Id as DeliveryManId,
//Case When Orders.PaymentMethod = 0 Then 'Pending' Else 'Paid' End As PaymentStatus,
//Stores.Name as StoreName,
//Stores.Id as StoreId,
//Stores.Location as StoreLocation,
//Users.FullName as CustomerName
//from Orders
//join Users on Users.ID = Orders.User_ID
//join StoreOrders on StoreOrders.Order_Id = Orders.Id
//join Stores on Stores.Id = StoreOrders.Store_Id
//where 
//Orders.IsDeleted = 0
//and Orders.Status = " + (int)OrderStatuses.ReadyForDelivery;
//                    #endregion

//                    SearchOrdersListViewModel responseModel = new SearchOrdersListViewModel { Orders = ctx.Database.SqlQuery<SearchOrdersViewModel>(query).ToList() };

//                    foreach (var order in responseModel.Orders)
//                    {
//                        var deliveryMen = ctx.DeliveryMen.Where(x => x.Location.Distance(order.StoreLocation) < Global.NearbyStoreRadius).ToList();

//                        foreach (var deliverer in deliveryMen)
//                        {
//                            order.DeliveryMen.Add(new DelivererOptionsViewModel { Id = deliverer.Id, Name = deliverer.FullName });
//                        }
//                    }

//                    //If a deliverer is in radius any of store in order. That deliverer will be selected.

//                    var duplicateOrders = responseModel.Orders.GroupBy(x => x.Id).Where(g => g.Count() > 1).Select(y => y.Key);

//                    var DuplicateDeliveryMenUnion = responseModel.Orders.Where(x => duplicateOrders.Contains(x.Id)).SelectMany(x1 => x1.DeliveryMen).Distinct(new DelivererOptionsViewModel.Comparer()).ToList();

//                    foreach (var order in responseModel.Orders.Where(x => duplicateOrders.Contains(x.Id)))
//                    {
//                        order.DeliveryMen = DuplicateDeliveryMenUnion;
//                    }

//                    return Ok(new CustomResponse<SearchOrdersListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });
//                }
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(Utility.LogError(ex));
//            }
//        }


        //[BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        //[HttpPost]
        //[Route("AssignOrdersToDeliverer")]
        //public async Task<IHttpActionResult> AssignOrdersToDeliverer(SearchOrdersListViewModel model)
        //{
        //    try
        //    {
        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return BadRequest(ModelState);
        //            }

        //            foreach (var order in model.Orders)
        //            {
        //                var existingOrder = ctx.Orders.Include(x => x.StoreOrders).FirstOrDefault(x => x.Id == order.Id);
        //                if (existingOrder != null)
        //                {
        //                    foreach (var storeOrder in existingOrder.StoreOrders)
        //                    {
        //                        storeOrder.Status = (int)OrderStatuses.AssignedToDeliverer;
        //                    }
        //                    existingOrder.DeliveryMan_Id = order.DeliveryManId;
        //                    existingOrder.Status = (int)OrderStatuses.AssignedToDeliverer;
        //                }
        //            }
        //            ctx.SaveChanges();
        //            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}


        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("SearchOrders")]
        public async Task<IHttpActionResult> SearchOrders(string StartDate, string EndDate, int? OrderStatusId, int? PaymentMethodId, int? PaymentStatusId, int? StoreId)
        {
            try
            {
                DateTime startDateTime;
                DateTime endDateTime;
                startDateTime = DateTime.ParseExact(StartDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                endDateTime = DateTime.ParseExact(EndDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                #region query
                var query = @"
select 
Orders.Id,
StoreOrders.Id as StoreOrder_Id,
Orders.OrderDateTime as CreatedOn,
Orders.Total as OrderTotal,
StoreOrders.Status as OrderStatus,
Orders.DeliveryMan_Id as DeliveryManId,
Case When Orders.PaymentMethod = 0 Then 'Pending' Else 'Paid' End As PaymentStatus,
Stores.BusinessName as StoreName,
Stores.Id as StoreId,
Stores.Location as StoreLocation,
Users.FullName as CustomerName
from Orders
join Users on Users.ID = Orders.User_ID
join StoreOrders on StoreOrders.Order_Id = Orders.Id
join Stores on Stores.Id = StoreOrders.Store_Id
where 
Orders.IsDeleted = 0
and 
 CAST(orders.OrderDateTime AS DATE) >= '" + startDateTime.Date + "' and CAST(orders.OrderDateTime as DATE) <= '" + endDateTime.Date + "'";
                #endregion

                if (OrderStatusId.HasValue)
                    query += " and orders.Status = " + OrderStatusId.Value;

                if (PaymentMethodId.HasValue)
                    query += " and orders.PaymentMethod = " + PaymentMethodId.Value;

                if (PaymentStatusId.HasValue)
                    query += " and orders.PaymentStatus = " + PaymentStatusId.Value;

                if (StoreId.HasValue)
                    query += " and Stores.Id = " + StoreId.Value;

                SearchOrdersListViewModel returnModel = new SearchOrdersListViewModel();

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    returnModel.Orders = ctx.Database.SqlQuery<SearchOrdersViewModel>(query).ToList();

                    foreach (var order in returnModel.Orders)
                    {
                        order.OrderStatusName = DunkeyDelivery.Utility.GetOrderStatusName(order.OrderStatus);
                    }
                    return Ok(new CustomResponse<SearchOrdersListViewModel> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpPost]
        [Route("ChangeOrderStatus")]
        public async Task<IHttpActionResult> ChangeOrderStatus(ChangeOrderStatusListBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    //Mark Statuses for StoreOrders
                    foreach (var order in model.Orders)
                    {
                        var existingStoreOrder = ctx.StoreOrders.FirstOrDefault(x => x.Id == order.StoreOrder_Id);
                        if (existingStoreOrder != null)
                        {
                            existingStoreOrder.Status = order.Status;
                        }
                    }
                    //Mark Statuses for Orders
                    foreach (var order in model.Orders)
                    {
                        var existingOrder = ctx.Orders.Include(x => x.StoreOrders).FirstOrDefault(x => x.Id == order.OrderId);
                        existingOrder.Status = existingOrder.StoreOrders.Min(x => x.Status);
                    }

                    ctx.SaveChanges();
                }
                return Ok(new CustomResponse<string> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("AddPackage")]
        public async Task<IHttpActionResult> AddPackage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                Package model = new Package();
                Package existingPackage = new Package();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }

                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                {
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                }
                model.Name = httpRequest.Params["Name"];
                model.Price =Convert.ToDouble(httpRequest.Params["Price"]);
                model.Description = httpRequest.Params["Description"];
                model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);
                model.Status = 0;
                if (httpRequest.Params["package_products"] != null)
                {
                    var packageProducts = JsonConvert.DeserializeObject<List<Package_Products>>(httpRequest.Params["package_products"]);

                    if (model.Id > 0)
                    {
                        foreach (var item in packageProducts)
                        {
                            //item.Product_Id = item.Id;
                            item.Id = item.PackageProductId;

                        }
                    }
                    else
                    {
                        foreach (var item in packageProducts)
                            item.Product_Id = item.Id;
                    }
                    model.Package_Products = packageProducts;
                }

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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Packages.Any(x => x.Store_Id == model.Store_Id && x.Name == model.Name && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Package already exist under same store" }
                            });
                        }
                    }
                    else
                    {
                        existingPackage = ctx.Packages.Include(x => x.Package_Products).FirstOrDefault(x => x.Id == model.Id);
                        if (existingPackage.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false || existingPackage.Store_Id != model.Store_Id)
                        {
                            if (ctx.Packages.Any(x => x.IsDeleted == false && x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Package with same name already exist under same store" }
                                });
                            }
                        }
                    }

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = Path.GetExtension(postedFile.FileName);
                            fileExtension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(fileExtension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > DunkeyDelivery.Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + DunkeyDelivery.Global.ImageSize }
                                });
                            }
                            else
                            {
                                //int count = 1;
                                //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + postedFile.FileName);

                                //while (File.Exists(newFullPath))
                                //{
                                //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + tempFileName + fileExtension);
                                //}
                                //postedFile.SaveAs(newFullPath);
                            }
                        }
                        //model.ImageUrl = ConfigurationManager.AppSettings["ProductImageFolderPath"] + Path.GetFileName(newFullPath);
                    }
                    #endregion

                    if (model.Id == 0)
                    {
                        ctx.Packages.Add(model);
                        ctx.SaveChanges();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["PackageImageFolderPath"] + model.Id + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        model.ImageUrl = ConfigurationManager.AppSettings["PackageImageFolderPath"] + model.Id + fileExtension;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (model.ImageDeletedOnEdit == false)
                            {
                                model.ImageUrl = existingPackage.ImageUrl;
                            }
                        }
                        else
                        {
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["PackageImageFolderPath"] + model.Id + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            model.ImageUrl = ConfigurationManager.AppSettings["PackageImageFolderPath"] + model.Id + fileExtension;
                        }

                        ctx.Entry(existingPackage).CurrentValues.SetValues(model);

                        foreach (var oldPP in existingPackage.Package_Products.ToList())
                        {
                            ctx.Package_Products.Remove(oldPP);
                        }

                        foreach (var packageProduct in model.Package_Products)
                        {
                            packageProduct.Package_Id = existingPackage.Id;
                            existingPackage.Package_Products.Add(packageProduct);

                            #region commented

                            //var originalPackageProduct = existingPackage.Package_Products.Where(c => c.Id == packageProduct.Id).SingleOrDefault();

                            //if (originalPackageProduct != null)
                            //{
                            //    // Yes -> Update scalar properties of child item
                            //    packageProduct.Package_Id = originalPackageProduct.Package_Id;
                            //    ctx.Entry(originalPackageProduct).CurrentValues.SetValues(packageProduct);
                            //}
                            //else
                            //{
                            //    // No -> It's a new child item -> Insert
                            //    packageProduct.Package_Id = existingPackage.Id;
                            //    existingPackage.Package_Products.Add(packageProduct);
                            //} 
                            #endregion
                        }
                        ctx.SaveChanges();
                    }

                    CustomResponse<Package> response = new CustomResponse<Package>
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

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("AddOffer")]
        public async Task<IHttpActionResult> AddOffer()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                Offer model = new Offer();
                Offer existingOffer = new Offer();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }

                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                {
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                }
                model.Name = httpRequest.Params["Name"];
                model.Title = model.Name;
                model.ValidFrom = DateTime.ParseExact(httpRequest.Params["ValidFrom"], "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                model.ValidUpto = DateTime.ParseExact(httpRequest.Params["ValidTo"], "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                //model.Price = Convert.ToDouble(httpRequest.Params["Price"]);
                model.Description = httpRequest.Params["Description"];
                model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);
                model.Status = "0";
                if (httpRequest.Params["offer_products"] != null)
                {
                    var offerProducts = JsonConvert.DeserializeObject<List<Offer_Products>>(httpRequest.Params["offer_products"]);

                    if (model.Id > 0)
                    {
                        foreach (var item in offerProducts)
                        {
                            //item.Product_Id = item.Id;
                            item.Id = item.OfferProductId;

                        }
                    }
                    else
                    {
                        foreach (var item in offerProducts)
                            item.Product_Id = item.Id;
                    }
                    model.Offer_Products = offerProducts;
                }

                if (httpRequest.Params["offer_packages"] != null)
                {
                    var offerPackages = JsonConvert.DeserializeObject<List<Offer_Packages>>(httpRequest.Params["offer_packages"]);

                    if (model.Id > 0)
                    {
                        foreach (var item in offerPackages)
                        {
                            //item.Product_Id = item.Id;
                            item.Id = item.OfferPackageId;

                        }
                    }
                    else
                    {
                        foreach (var item in offerPackages)
                            item.Package_Id = item.Id;
                    }
                    model.Offer_Packages = offerPackages;
                }
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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Offers.Any(x => x.Store_Id == model.Store_Id && x.Name == model.Name && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Offer with same name already exist under same store" }
                            });
                        }
                    }
                    else
                    {
                        existingOffer = ctx.Offers.Include(x => x.Offer_Products).Include(x => x.Offer_Packages).FirstOrDefault(x => x.Id == model.Id);
                        if (existingOffer.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false || existingOffer.Store_Id != model.Store_Id)
                        {
                            if (ctx.Offers.Any(x => x.IsDeleted == false && x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Offer with same name already exist under same store" }
                                });
                            }
                        }
                    }

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = Path.GetExtension(postedFile.FileName);
                            fileExtension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(fileExtension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > DunkeyDelivery.Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + DunkeyDelivery.Global.ImageSize }
                                });
                            }
                            else
                            {
                                //int count = 1;
                                //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + postedFile.FileName);

                                //while (File.Exists(newFullPath))
                                //{
                                //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + tempFileName + fileExtension);
                                //}
                                //postedFile.SaveAs(newFullPath);
                            }
                            //model.ImageUrl = ConfigurationManager.AppSettings["ProductImageFolderPath"] + Path.GetFileName(newFullPath);
                        }
                    }
                    #endregion

                    if (model.Id == 0)
                    {
                        ctx.Offers.Add(model);
                        ctx.SaveChanges();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["OfferImageFolderPath"] + model.Id + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        model.ImageUrl = ConfigurationManager.AppSettings["OfferImageFolderPath"] + model.Id + fileExtension;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (model.ImageDeletedOnEdit == false)
                            {
                                model.ImageUrl = existingOffer.ImageUrl;
                            }
                        }
                        else
                        {
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["OfferImageFolderPath"] + model.Id + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            model.ImageUrl = ConfigurationManager.AppSettings["OfferImageFolderPath"] + model.Id + fileExtension;
                        }

                        ctx.Entry(existingOffer).CurrentValues.SetValues(model);

                        //Delete and Insert OfferProducts

                        foreach (var oldOP in existingOffer.Offer_Products)
                        {
                            oldOP.IsDeleted = true;
                        }

                        foreach (var newOP in model.Offer_Products)
                        {
                            var oldOP = ctx.Offer_Products.FirstOrDefault(x => x.Id == newOP.Id);
                            if (oldOP == null)
                            {
                                newOP.Offer_Id = existingOffer.Id;
                                existingOffer.Offer_Products.Add(newOP);
                            }
                            else
                            {
                                newOP.Offer_Id = existingOffer.Id;
                                ctx.Entry(ctx.Offer_Products.FirstOrDefault(x => x.Id == oldOP.Id)).CurrentValues.SetValues(newOP);
                            }
                        }

                        //Delete and Insert OfferPackages
                        foreach (var oldOP in existingOffer.Offer_Packages)
                        {
                            oldOP.IsDeleted = true;
                        }

                        foreach (var newOP in model.Offer_Packages)
                        {
                            var oldOP = ctx.Offer_Packages.FirstOrDefault(x => x.Id == newOP.Id);
                            if (oldOP == null)
                            {
                                newOP.Offer_Id = existingOffer.Id;
                                existingOffer.Offer_Packages.Add(newOP);
                            }
                            else
                            {
                                newOP.Offer_Id = existingOffer.Id;
                                ctx.Entry(ctx.Offer_Packages.FirstOrDefault(x => x.Id == oldOP.Id)).CurrentValues.SetValues(newOP);
                            }
                        }
                        ctx.SaveChanges();
                    }



                    CustomResponse<Offer> response = new CustomResponse<Offer>
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

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(AdminSetPasswordBindingModel model)
        {
            try
            {
                var userEmail = User.Identity.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new Exception("User Email is empty in user.identity.name.");
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var user = ctx.Admins.FirstOrDefault(x => x.Email == userEmail && x.Password == model.OldPassword);
                    if (user != null)
                    {
                        user.Password = model.NewPassword;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid old password." } });


                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IHttpActionResult> GetUsers()
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    return Ok(new CustomResponse<SearchUsersViewModel>
                    {
                        Message = Utility.Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new SearchUsersViewModel
                        {
                            Users = ctx.Users.ToList()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(LogError(ex));
            }
        }

    }
}
