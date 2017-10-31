using DunkeyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DAL;
using DunkeyAPI.ViewModels;
using DunkeyDelivery;
using System.Web.Http.ModelBinding;
using System.Web;
using System.IO;
using System.Configuration;
using WebApplication1.Models;
using Microsoft.Owin.Security.OAuth;
using Nexmo.Api;
using System.Net.Mail;
using System.Data.Entity;
using AutoMapper;
using static DunkeyAPI.Utility.Global;

namespace DunkeyAPI.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        [AllowAnonymous]
        [Route("Test")]
        [HttpGet]
        public async Task<string> Test()
        {
            return "Hello World";
        }

        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login(LoginBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var userModel = ctx.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
                    if (userModel != null)
                    {
                        using (UserViewModel userViewModel = new UserViewModel(userModel))
                        {
                            await userModel.GenerateToken(Request);
                            
                            CustomResponse<User> response = new CustomResponse<User> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = userModel };
                            return Ok(response);
                        }
                    }
                    else
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "Invalid Email or Password" }
                        });
                    //return CustomResponse("Invalid UserName or Password");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }
        // POST api/Account/ForgetPassword
        [Route("ForgetPassword")]
        public async Task<IHttpActionResult> ForgetPassword(ForgetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (DunkeyContext ctx = new DunkeyContext())
            {
                if (ctx.Users.Any(x => x.Email != model.Email))
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Email Address Not Found",
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Result = new Error { ErrorMessage = "User With Email Not Found" }

                    });
                }
                else
                {




                }

            }



            return Ok();
        }


        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
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
                    if (ctx.Users.Any(x => x.Email == model.Email))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "User with email already exists" }
                        });
                        //return BadRequest("User with the same email already exists.");

                        //return Request.CreateResponse(HttpStatusCode.OK, new Error { ErrorCode = "400 Conflict", Message = "User with email already exist"});
                    }
                    else
                    {
                        User userModel = new User
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            Password = model.Password,
                            FullName = model.FirstName + " " + model.LastName,
                            Status = (int)Global.StatusCode.NotVerified,
                            Phone = model.Phone,
                            Role = model.Role

                        };

                        ctx.Users.Add(userModel);
                        ctx.SaveChanges();
                        using (UserViewModel userViewModel = new UserViewModel(userModel))
                        {
                            await userModel.GenerateToken(Request);
                            CustomResponse<User> response = new CustomResponse<User> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = userModel };
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
            //var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            //IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}

            //return Ok();
        }

        [Route("RegisterWithImage")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RegisterWithImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                RegisterBindingModel model = new RegisterBindingModel();
                model.FirstName = httpRequest.Params["FirstName"];
                model.LastName = httpRequest.Params["LastName"];
                model.Email = httpRequest.Params["Email"];
                model.Password = httpRequest.Params["Password"];
                model.Phone = httpRequest.Params["Phone"];
                model.ConfirmPassword = httpRequest.Params["ConfirmPassword"];
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
                    if (ctx.Users.Any(x => x.Email == model.Email))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "User with email already exists" }
                        });
                        //return BadRequest("User with the same email already exists.");

                        //return Request.CreateResponse(HttpStatusCode.OK, new Error { ErrorCode = "400 Conflict", Message = "User with email already exist"});
                    }
                    else
                    {
                        #region ImageSaving
                        var postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            //int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            //var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
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
                                newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + postedFile.FileName);

                                while (File.Exists(newFullPath))
                                {
                                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + tempFileName + extension);
                                }

                                postedFile.SaveAs(newFullPath);
                            }
                        }
                        #endregion

                        //var RequestUriString = Request.RequestUri.ToString();
                        //var baseUrl = RequestUriString.Substring(0, RequestUriString.IndexOf("api"));
                        User userModel = new User
                        {
                            Email = model.Email,
                            Password = model.Password,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Status = (int)Global.StatusCode.NotVerified,
                            Role = model.Role,
                            Phone = model.Phone,
                            ProfilePictureUrl = DunkeyDelivery.Utility.BaseUrl + ConfigurationManager.AppSettings["UserImageFolderPath"] + Path.GetFileName(newFullPath)
                        };

                        ctx.Users.Add(userModel);
                        ctx.SaveChanges();
                        using (UserViewModel userViewModel = new UserViewModel(userModel))
                        {
                            await userModel.GenerateToken(Request);
                            CustomResponse<User> response = new CustomResponse<User> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = userModel };
                            return Ok(response);
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }
        // POST api/Account/ChangePassword
        [DunkeyDelivery.Authorize]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            try
            {

                var userEmail = User.Identity.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new Exception("User Email Field Is Empty");
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    ctx.Users.FirstOrDefault(x => x.Email == userEmail).Password = model.NewPassword;
                    ctx.SaveChanges();
                }
                return Content(HttpStatusCode.OK, new MessageViewModel { StatusCode = "200 OK", Details = "Password Updated Successfully." });

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }
        // POST api/Account/Logout
        [System.Web.Http.Authorize]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            HttpContext.Current.GetOwinContext().Authentication.SignOut(OAuthDefaults.AuthenticationType);
            return Content(HttpStatusCode.OK, new MessageViewModel { StatusCode = "200 OK", Details = "User Logged Out Successfully" });
        }


        [HttpPost]
        [Route("ResetPassword")]
        public IHttpActionResult ResetPassword(ResetPassword model)
        {
            
            try
            {

                if (!ModelState.IsValid) { 
                    return BadRequest(ModelState);
                }
                

                using (DunkeyContext ctx=new DunkeyContext())
                {
                    var userModel = ctx.ForgotPasswordTokens.Include(x=>x.User).FirstOrDefault(x => x.Code == model.Code);

                    if (userModel != null)
                    {
                        if (DateTime.Now.Subtract(userModel.CreatedAt).Hours >Global.VerificationCodeTimeOut){
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Validation Code Expires" }
                            });
                        }

                        userModel.User.Password = model.Password;
                        ctx.SaveChanges();
                        CustomResponse<ForgetPasswordTokens> response = new CustomResponse<ForgetPasswordTokens> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = userModel };
                        return Ok(response);
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "NotFound",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Result = new Error { ErrorMessage = "Invalid Server Error" }
                        });
                    }

                }
              

                
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("SendVerificationSms")]
        public IHttpActionResult SendVerificationSms(PhoneBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nexmoVerifyResponse = NumberVerify.Verify(new NumberVerify.VerifyRequest { brand = "INGIC", number = model.Phone });

                if (nexmoVerifyResponse.status == "0")
                    return Content(HttpStatusCode.OK, new CustomResponse<NumberVerify.VerifyResponse> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = nexmoVerifyResponse });
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "Verification SMS failed due to some reason." } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }



        [HttpPost]
        [Route("VerifySmsCode")]
        public IHttpActionResult VerifySmsCode(PhoneVerificationModel model)
        {
            try
            {
                var userEmail = User.Identity.Name;

                if (string.IsNullOrEmpty(userEmail))
                    throw new Exception("User Email is empty in user.identity.name.");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nexmoCheckResponse = NumberVerify.Check(new NumberVerify.CheckRequest { request_id = model.request_id, code = model.Code });

                if (nexmoCheckResponse.status == "0")
                {
                    using (DunkeyContext ctx = new DunkeyContext())
                    {
                        ctx.Users.FirstOrDefault(x => x.Email == userEmail).Status = (int)Global.StatusCode.Verified;
                        ctx.SaveChanges();
                    }
                    return Content(HttpStatusCode.OK, new MessageViewModel { Details = "Account Verified Successfully." });
                }
                else
                    return Content(HttpStatusCode.OK, new CustomResponse<NumberVerify.CheckResponse> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = nexmoCheckResponse });
                //return Content(HttpStatusCode.InternalServerError, new CustomResponse<Error> { StatusCode = "500 InternalServerError", Details = new Error { ErrorMessage = "Account"} });
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("SendAppLink")]
        public IHttpActionResult SendAppLink(PhoneBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nexmoVerifyResponse = SMS.Send(new SMS.SMSRequest
                {
                    from = "03455249413",
                    to = model.Phone,
                    text = "INGIC : Donwload Phone Application From Google Store"

                });

                //if (nexmoVerifyResponse)
                //    return Content(HttpStatusCode.OK, new CustomResponse<NumberVerify.VerifyResponse> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = nexmoVerifyResponse });
                //else
                //{
                //    return Content(HttpStatusCode.OK, new CustomResponse<Error> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "Verification SMS failed due to some reason." } });
                //}
                return Content(HttpStatusCode.OK, new CustomResponse<SMS.SMSResponse> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = nexmoVerifyResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("ResetPasswordThroughEmail")]
        public async Task<IHttpActionResult> ResetPasswordThroughEmail(string email)
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    //UserForgetToken userToken = new UserForgetToken();
                    var user = ctx.Users
                        .Where(x=>x.Email==email)
                        .Include(x=>x.ForgetPasswordToken).FirstOrDefault();

                    if (user != null)
                    {
                        var nexmoVerifyResponse = NumberVerify.Verify(new NumberVerify.VerifyRequest { brand = "INGIC", number = user.Phone });
                        if (nexmoVerifyResponse.status == "0")
                            return Content(HttpStatusCode.OK, new CustomResponse<NumberVerify.VerifyResponse> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = nexmoVerifyResponse });
                        else
                        {
                            nexmoVerifyResponse.error_text = "Mobile number you provided for this email is invalid.";
                            return Content(HttpStatusCode.OK, new CustomResponse<NumberVerify.VerifyResponse> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = nexmoVerifyResponse });
                        }

                        return Ok(new CustomResponse<User> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = user });
                    }

                    //var r = from User user1 in ctx.Users
                    //        from token in user1.ForgetPasswordToken
                    //        where user1.Email == email
                    //        select new { name = user1.FirstName, token = token.Code}
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Conflict",
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Result = new Error { ErrorMessage = "User with email doesn't exist" }
                    });



                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetUserByEmail")]
        public IHttpActionResult GetUserByEmail(string Email)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var res = ctx.Users
                        .Where(x => x.Email == Email)
                        .FirstOrDefault();

                    UserViewModel userModel =new UserViewModel(res);
                    CustomResponse<UserViewModel> response = new CustomResponse<UserViewModel>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = userModel
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [AllowAnonymous]
        [Route("SubmitContactUs")]
        [HttpPost]
        public async Task<IHttpActionResult> SubmitContactUs(ContactUsBindingModel model)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                using (DunkeyContext ctx = new DunkeyContext())
                {
                    ContactUs contactModel = new ContactUs
                    {
                        Name = model.Name,
                        Email=model.Email,
                        Phone=model.Phone,
                        Message=model.Message


                    };

                    ctx.ContactUs.Add(contactModel);
                    ctx.SaveChanges();
                    
                    CustomResponse<string> response = new CustomResponse<string> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result ="Query Submitted Successfully"  };
                    return Ok(response);



                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        [AllowAnonymous]
        [Route("EditProfile")]
        [HttpPost]
        public async Task<IHttpActionResult> EditProfile(ProfileViewModel model)
         {
          
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var User = ctx.Users.FirstOrDefault(x => x.Email == model.EmailAddress);

                    if (string.IsNullOrEmpty(model.Password) && string.IsNullOrEmpty(model.ConfirmPassword))
                    {
                        User.FirstName = model.FName;
                        User.LastName = model.LName;
                        User.FullName = model.FName + " " + model.LName;
                    }else
                    {
                        User.FirstName = model.FName;
                        User.LastName = model.LName;
                        User.Password = model.Password;
                    }
                    ctx.SaveChanges();
                    CustomResponse<User> response = new CustomResponse<User> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = User };
                    return Ok(response);
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }
          
        }


        [AllowAnonymous]
        [Route("AddAddress")]
        [HttpPost]
        public async Task<IHttpActionResult> AddAddress(AddressViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                using (DunkeyContext ctx = new DunkeyContext())
                {
                var addressCount=ctx.UserAddresses.Where(x => x.FullAddress == model.FullAddress && x.IsDeleted==false).Count();

                    if (addressCount > 0) {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "User with address already exists." }
                        });
                    }
                    else
                    {
                        UserAddress addressModel = new UserAddress
                        {
                            User_ID = model.User_ID,
                            City = model.City,
                            FullAddress = model.FullAddress,
                            PostalCode = model.PostalCode,
                            State = model.State,
                            Telephone = model.Telephone,
                            IsPrimary = false,
                            IsDeleted = false,
                        };

                            ctx.UserAddresses.Add(addressModel);
                            ctx.SaveChanges();
                        CustomResponse<UserAddress> response = new CustomResponse<UserAddress> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = addressModel };
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
        [Route("GetUserAddresses")]
        public IHttpActionResult GetUserAddresses(int User_id)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var res = ctx.UserAddresses
                        .Where(x => x.User_ID == User_id && x.IsDeleted==false).ToList();
                  

                    Addresses addressModel = new Addresses();
                    var f = Mapper.Map<List<AddressViewModel>>(res);
                    addressModel.addresses = f;
                    CustomResponse<Addresses> response = new CustomResponse<Addresses>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = addressModel
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        [AllowAnonymous]
        [Route("RemoveAddress")]
        [HttpGet]
        public async Task<IHttpActionResult> RemoveAddress(int address_id,int User_Id)
        {

            try
            {
               
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var UserAddress = ctx.UserAddresses.FirstOrDefault(x => x.User_ID ==User_Id && x.Id==address_id);

                    if (UserAddress != null)
                    {
                        UserAddress.IsDeleted = true;
                       
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "User with this address deleted." }
                        });
                    }

                    ctx.SaveChanges();
                    CustomResponse<string> response = new CustomResponse<string> { Message = Global.SuccessMessage, StatusCode = (int)HttpStatusCode.OK, Result = "Address removed successfully" };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        [AllowAnonymous]
        [Route("AddCreditCard")]
        [HttpPost]
        public async Task<IHttpActionResult> AddCreditCard(CreditCards model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var creditCard = ctx.CreditCards.Where(x => x.CCNo == model.CCNo).FirstOrDefault();
                    if (creditCard==null)
                    {
                        CreditCard creditcardModel = new CreditCard
                        {
                            User_ID = model.User_ID,
                            BillingCode=model.BillingCode,
                            CCNo=model.CCNo,
                            CCV=model.CCV,
                            ExpiryDate=model.ExpiryDate
                        


                        };

                        ctx.CreditCards.Add(creditcardModel);
                        ctx.SaveChanges();


                        CustomResponse<CreditCard> response = new CustomResponse<CreditCard> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = creditcardModel };
                        return Ok(response);
                    }else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "User with this credit card already exists." }
                        });
                    }


                }
            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }

        [HttpGet]
        [Route("GetUserCreditCards")]
        public IHttpActionResult GetUserCreditCards(int User_id)
        {
            try
            {

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var res = ctx.CreditCards
                        .Where(x => x.User_ID == User_id).ToList();


                    UserCreditCard creditcardModel = new UserCreditCard();
                    var f = Mapper.Map<List<CreditCards>>(res);
                    creditcardModel.CreditCards = f;
                    CustomResponse<UserCreditCard> response = new CustomResponse<UserCreditCard>
                    {
                        Message = "Success",
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = creditcardModel
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(DunkeyDelivery.Utility.LogError(ex));
            }

        }


        // admin panel services 
        /// <summary>
        /// Login for web admin panel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("WebPanelLogin")]
        [HttpPost]
        public async Task<IHttpActionResult> WebPanelLogin(WebLoginBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    DAL.Admin adminModel;

                    adminModel = ctx.Admins.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

                    if (adminModel != null)
                    {
                        await adminModel.GenerateToken(Request);
                        CustomResponse<Admin> response = new CustomResponse<Admin> { Message = ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = adminModel };
                        return Ok(response);
                    }
                    else
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "Invalid Email or Password" }
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
