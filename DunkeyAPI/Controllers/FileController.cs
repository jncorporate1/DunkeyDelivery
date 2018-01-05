using DAL;
using DunkeyDelivery;
using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    [RoutePrefix("api/File")]
    public class FileController : ApiController
    {

        [HttpPost]
        [Route("ImportProducts")]
        public async Task<IHttpActionResult> ImportProducts()
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var ValidationCount = 0;
                    var httpRequest = HttpContext.Current.Request;
                    string newFullPath = string.Empty;
                    string fileNameOnly = string.Empty;

                    if (httpRequest.Files.Count > 1)
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "UnsupportedMediaType",
                            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                            Result = new Error { ErrorMessage = "Multiple files are not supported, please upload one image." }
                        });
                    }

                    #region ImageSaving
                    var postedFile = httpRequest.Files[0];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        //int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".xlsx",".xls" };
                   
                        var ext = Path.GetExtension(postedFile.FileName);
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "UnsupportedMediaType",
                                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                Result = new Error { ErrorMessage = "Please Upload file of type .xlsx or .xls" }
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
                            //int count = 1;
                            //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                            //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + postedFile.FileName);

                            //while (File.Exists(newFullPath))
                            //{
                            //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                            //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + tempFileName + extension);
                            //}

                            //postedFile.SaveAs(newFullPath);
                            List<Product> ProductToImport = new List<Product>();
                            Product SingleProduct = new Product();
                            Error ReturnError = new Error();
                            var ErrorCount = 0;
                            
                            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                            
                            var workbook = ExcelFile.Load(postedFile.InputStream, extension == ".xls" ? (LoadOptions)LoadOptions.XlsDefault : (LoadOptions)LoadOptions.XlsxDefault);

                            var worksheet = workbook.Worksheets.ActiveWorksheet;

                            foreach (var allocatedCell in worksheet.Rows)
                            {
                                foreach (ExcelCell cell in allocatedCell.AllocatedCells)
                                {
                                    if (cell.Row.Index == 0 )
                                    {
                                        if (cell.Column.Index == 0 && cell.Value.ToString().Contains("Name"))
                                        {
                                            
                                            ValidationCount++;
                                        }
                                        else if (cell.Column.Index == 1 && cell.Value.ToString().Contains("Price"))
                                        {
                                            ValidationCount++;
                                        }
                                        else if (cell.Column.Index == 2 && cell.Value.ToString().Contains("Description"))
                                        {
                                            ValidationCount++;
                                        }
                                        else if (cell.Column.Index == 3 && cell.Value.ToString().Contains("Status"))
                                        {
                                            ValidationCount++;
                                        }
                                        else if (cell.Column.Index == 4 && cell.Value.ToString().Contains("Category_Id"))
                                        {
                                            ValidationCount++;
                                        }
                                        else if (cell.Column.Index == 5 && cell.Value.ToString().Contains("Store_Id"))
                                        {
                                            ValidationCount++;
                                        }
                                        else if (cell.Column.Index == 6 && cell.Value.ToString().Contains("Size"))
                                        {
                                            ValidationCount++;
                                        }
                                        else
                                        {

                                        }
                                    }else
                                    {
                                        if (ValidationCount == 7)
                                        {
                                            if (cell.Column.Index == 0)
                                            {
                                                SingleProduct.Name =Convert.ToString(cell.Value); 
                                            }
                                            else if (cell.Column.Index == 1)
                                            {
                                                SingleProduct.Price =Convert.ToDouble(cell.Value);
                                            }
                                            else if (cell.Column.Index == 2)
                                            {
                                                SingleProduct.Description = Convert.ToString(cell.Value);
                                            }
                                            else if (cell.Column.Index == 3)
                                            {
                                                SingleProduct.Status = Convert.ToInt16(cell.Value);                     
                                            }
                                            else if (cell.Column.Index == 4 )
                                            {
                                                SingleProduct.Category_Id = Convert.ToInt32(cell.Value);
                                            }
                                            else if (cell.Column.Index == 5 )
                                            {
                                                SingleProduct.Store_Id = Convert.ToInt32(cell.Value);
                                            }
                                            else if (cell.Column.Index == 6)
                                            {
                                                SingleProduct.Size = Convert.ToString(cell.Value);
                                                SingleProduct.IsDeleted = false;
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                }
                                if (allocatedCell.Index != 0)
                                {
                                    ProductToImport.Add(SingleProduct);
                                }
                            }


                            foreach (var product in ProductToImport)
                            {
                                if (!ctx.Categories.Any(x=>x.Id==product.Category_Id && x.Store_Id == product.Store_Id))
                                {
                                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                    {
                                        Message = "Conflict",
                                        StatusCode = (int)HttpStatusCode.Conflict,
                                        Result = new Error { ErrorMessage = "Invalid Store_Id "+product.Store_Id+" or Category_Id " + product.Category_Id}
                                    });
                                }

                                if (ctx.Products.Any(x => x.Name.Contains(product.Name)))
                                {
                                    ReturnError.ErrorMessage ="Product by name '"+product.Name + "' already exists.";
                                    ErrorCount++;
                                }
                            }

                            if (ErrorCount == 0 && ProductToImport.Count>0)
                            {
                                ctx.Products.AddRange(ProductToImport);
                                ctx.SaveChanges();

                            }
                            else
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = ReturnError
                                });
                            }
                        }
                    }
                    #endregion
                    

                    CustomResponse<string> response = new CustomResponse<string>
                    {
                        Message = ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = "Products added successfully. "
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                StatusCode(DunkeyDelivery.Utility.LogError(ex));
               
                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                {
                    Message = "Conflict",
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Result =new Error { ErrorMessage="Invalid Store_Id or Category_Id" }
                });

            }
        }
    }
}
