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
    [RoutePrefix("api/Driver")]
    public class DriverController : ApiController
    {
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(DriverBindingModel model)
        {
            model.Role = 0;
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (ctx.Drivers.Any(x => x.Email == model.Email))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "Driver with email already exists" }
                        });
                        //return BadRequest("User with the same email already exists.");

                        //return Request.CreateResponse(HttpStatusCode.OK, new Error { ErrorCode = "400 Conflict", Message = "User with email already exist"});
                    }
                    else
                    {
                        Driver driverModel = new Driver
                        {
                           FullName=model.FullName,
                           Email=model.Email,
                           Phone=model.Phone,
                           City=model.City,
                           VehicleType=model.VehicleType,
                           HearFrom=model.HearFrom
                           

                        };

                        ctx.Drivers.Add(driverModel);
                        ctx.SaveChanges();
                        using (DriverViewModel driverViewModel = new DriverViewModel(driverModel))
                        {
                            //driverViewModel.Token = await Utility.GenerateToken(driverModel.Email, driverModel.Password, Request);
                            CustomResponse<DriverViewModel> response = new CustomResponse<DriverViewModel> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = driverViewModel };
                            return Ok(response);
                            //return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
       
        }


    }
}
