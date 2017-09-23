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
    [RoutePrefix("api/Ride")]
    public class RideController : ApiController
    {
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(RideBindingModel model)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                using (DunkeyContext ctx = new DunkeyContext())
                {
                    if (ctx.Rider.Any(x => x.Email == model.Email))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "Rider with email already exists" }
                        });

                    }
                    else
                    {
                        Rider rideModel = new Rider()
                        {
                            FullName = model.FullName,
                            BusinessName= model.BusinessName,
                            Email=model.Email,
                            BusinessType = model.BusinessType,
                            Status = (int)Global.StatusCode.NotVerified,
                            Phone = model.Phone,
                            ZipCode = model.ZipCode
                            

                        };

                        ctx.Rider.Add(rideModel);
                        ctx.SaveChanges();
                        using (RideViewModel rideViewModel = new RideViewModel(rideModel))
                        {
                            //userViewModel.Token = await Utility.GenerateToken(userModel.Email, userModel.Password, Request);
                            CustomResponse<RideViewModel> response = new CustomResponse<RideViewModel> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = rideViewModel };
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
