using DAL;
using DunkeyAPI.Models;
using DunkeyAPI.ViewModels;
using DunkeyDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/Merchant")]
    public class MerchantController : ApiController
    {
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(MerchantBindingModel model)
        {
            model.Role = 1;
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (ctx.Admins.Any(x => x.Email == model.Email))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "Merchant with email already exists" }
                        });
                        //return BadRequest("User with the same email already exists.");

                        //return Request.CreateResponse(HttpStatusCode.OK, new Error { ErrorCode = "400 Conflict", Message = "User with email already exist"});
                    }
                    else
                    {
                        Admin adminModel = new Admin
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            //Password = "12313123", //model.Password,
                            FullName = model.FirstName + " " + model.LastName,
                            BusinessName=model.BusinessName,
                            BusinessType=model.BusinessType,
                            Status = (int)Global.StatusCode.NotVerified,
                            Phone = model.Phone,
                            Role = model.Role

                        };

                        ctx.Admins.Add(adminModel);
                        ctx.SaveChanges();
                        using (AdminViewModel adminViewModel = new AdminViewModel(adminModel))
                        {
                            //userViewModel.Token = await Utility.GenerateToken(userModel.Email, userModel.Password, Request);
                            CustomResponse<AdminViewModel> response = new CustomResponse<AdminViewModel> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = adminViewModel };
                            return Ok(response);
                            //return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
    
        }

    }
}
