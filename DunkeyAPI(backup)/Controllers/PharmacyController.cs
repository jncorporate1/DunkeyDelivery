using DAL;
using DunkeyAPI.BindingModels;
using DunkeyAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Pharmacy")]
    public class PharmacyController : ApiController
    {
        [AllowAnonymous]
        [Route("SubmitPharmacyRequest")]
        [HttpPost]
        public async Task<IHttpActionResult> SubmitPharmacyRequest(PharmacyRequestBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else if (model.Product_Ids.Count == 0)
                {
                    return Ok(new CustomResponse<Error> { Message = Utility.Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Please add medicines" } });
                }
                
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    PharmacyRequest pharmModel = new PharmacyRequest
                    {
                        Delivery_Address = model.Delivery_Address,
                        Delivery_City = model.Delivery_City,
                        Delivery_Phone = model.Delivery_Phone,
                        Delivery_State = model.Delivery_State,
                        Delivery_Zip = model.Delivery_Zip,
                        Doctor_FirstName = model.Doctor_FirstName,
                        Doctor_LastName = model.Doctor_LastName,
                        Doctor_Phone = model.Doctor_Phone,
                        Gender = model.Gender,
                        Patient_DOB = DateTime.ParseExact(model.Patient_DOB, "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
                        Patient_FirstName = model.Patient_FirstName,
                        Patient_LastName = model.Patient_LastName,
                        CreatedDate = DateTime.Now
                    };

                    foreach (var productId in model.Product_Ids)
                    {
                        pharmModel.PharmacyRequest_Products.Add(new PharmacyRequest_Products { Product_Id = productId.Value });
                    }

                    ctx.PharmacyRequest.Add(pharmModel);
                    ctx.SaveChanges();
                    return Ok(new CustomResponse<PharmacyRequest> { Message = Utility.Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("GetSinglePharmacyRequest")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSinglePharmacyRequest(int Id)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    #region PharmacyQuery
                    var PharmacyQuery = @"
select 
Id,
Doctor_FirstName, 
Doctor_LastName,
Doctor_Phone,
Patient_FirstName,
Patient_LastName, 
Patient_DOB, 
Gender,
Delivery_Address,
Delivery_City,
Delivery_State,
Delivery_Zip,
Delivery_Phone, 
CreatedDate
from 
PharmacyRequests
where Id = "+Id + @" and 
isdeleted = 0 
"; 
                    #endregion

                    var pharmacyRequest =  ctx.Database.SqlQuery<PharmacyRequestViewModel>(PharmacyQuery).Single();

                    #region PharmacyProductQuery
                    var PharmacyProductQuery = @"

select 
Products.Id,
Products.Name
from PharmacyRequest_Products
join Products on Products.Id = PharmacyRequest_Products.Product_Id 
Where PharmacyRequest_Products.PharmacyRequest_Id = " + pharmacyRequest.Id;
                    #endregion

                    var pharmacyRequestProducts = ctx.Database.SqlQuery<PharmacyProductViewModel>(PharmacyProductQuery).ToList();

                    foreach (var product in pharmacyRequestProducts)
                    {
                        pharmacyRequest.PharmacyProducts.Add(product);
                    }

                    return Ok(new CustomResponse<PharmacyRequestViewModel>
                    {
                        Message = Utility.Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = pharmacyRequest
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [DunkeyDelivery.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("GetPharmacyRequests")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPharmacyRequests()
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    return Ok(new CustomResponse<PharmacyRequestsViewModel>
                    {
                        Message = Utility.Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new PharmacyRequestsViewModel { PharmacyRequests = ctx.PharmacyRequest.Where(x => x.IsDeleted == false).ToList() }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [AllowAnonymous]
        [Route("SubmitPharmacyRequestMobile")]
        [HttpPost]
        public async Task<IHttpActionResult> SubmitPharmacyRequestMobile(PharmacyRequestMobileBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else if (model.Product_Ids.Count == 0 && string.IsNullOrEmpty(model.Product_Id))
                {
                    return Ok(new CustomResponse<Error> { Message = Utility.Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Please add medicines" } });
                }

                if (!string.IsNullOrEmpty(model.Product_Id))
                {
                    var ProductIds = model.Product_Id.Split(',').Select(Int32.Parse).ToList();
                    foreach (var item in ProductIds)
                    {
                        model.Product_Ids.Add(item);
                    }
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {

                    UserAddress UserAddress = new UserAddress();

                    UserAddress = ctx.UserAddresses.FirstOrDefault(x => x.User_ID == model.User_Id && x.IsDeleted == false && x.IsPrimary == true);
                    if(UserAddress == null)
                    {
                        UserAddress = ctx.UserAddresses.Where(x => x.User_ID == model.User_Id && x.IsDeleted == false).OrderByDescending(x=>x.Id).FirstOrDefault();
                    }

                    if(UserAddress == null)
                    {
                        return Ok(new CustomResponse<Error> { Message = Utility.Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User addresses not found." } });
                    }
                    PharmacyRequest pharmModel = new PharmacyRequest
                    {
                        Delivery_Address = UserAddress.FullAddress,
                        Delivery_City = UserAddress.City,
                        Delivery_Phone = UserAddress.Telephone,
                        Delivery_State = UserAddress.State,
                        Delivery_Zip = UserAddress.PostalCode,
                        Doctor_FirstName = model.Doctor_FirstName,
                        Doctor_LastName = model.Doctor_LastName,
                        Doctor_Phone = model.Doctor_Phone,
                        Gender = model.Gender,
                        Patient_DOB =Convert.ToDateTime(model.Patient_DOB),
                        Patient_FirstName = model.Patient_FirstName,
                        Patient_LastName = model.Patient_LastName,
                        CreatedDate = DateTime.Now
                    };

                    foreach (var productId in model.Product_Ids)
                    {
                        pharmModel.PharmacyRequest_Products.Add(new PharmacyRequest_Products { Product_Id = productId.Value });
                    }

                    ctx.PharmacyRequest.Add(pharmModel);
                    ctx.SaveChanges();
                    return Ok(new CustomResponse<PharmacyRequest> { Message = Utility.Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }
    }
}
