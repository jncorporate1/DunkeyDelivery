using DunkeyDelivery.Models;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DunkeyDelivery.Controllers
{
    public class GmailController : Controller
    {
        // GET: Gmail
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> GmailCallback(string FirstName, string LastName, string Email, string ImageUrl)
        {
            try
            {
                string email = Email;
                TempData["email"] = Email;
                TempData["first_name"] = FirstName;
                TempData["lastname"] = LastName;
                TempData["picture"] = ImageUrl;
                LoginViewModel model = new LoginViewModel();
                model.Email = Email;
                model.Password = string.Empty;
                model.FirstName = FirstName;
                model.LastName = LastName;
                model.ProfilePictureUrl = ImageUrl;
                model.Role = 6;


                var response = await ApiCall<LoginViewModel>.CallApi("api/User/SocialLogin", model);
                if (response is Error)
                {
                    ModelState.AddModelError("", (response as Error).ErrorMessage);
                    return View("Login", model);
                }
                else
                {

                    ClaimsIdentity identity;
                    var userModel = response.GetValue("Result").ToObject<UserViewModel>();
                    var Role = userModel.Role;
                    model.Id = userModel.ID;
                    model.FirstName = userModel.FirstName;
                    model.LastName = userModel.LastName;

                    if (userModel.Role == Convert.ToInt32(Utility.RoleTypes.Gmail))
                    {
                        identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                        identity.AddClaim(new Claim("username", model.Email));
                        identity.AddClaim(new Claim("FullName", userModel.FullName));
                        if (string.IsNullOrEmpty(userModel.ProfilePictureUrl))
                        {
                            userModel.ProfilePictureUrl = ConfigurationManager.AppSettings["WebBaseUrl"] + "/DefaultImages/DefaultUserImage.png";
                        }
                        identity.AddClaim(new Claim("ProfilePictureUrl", userModel.ProfilePictureUrl));
                        identity.AddClaim(new Claim(ClaimTypes.Name, model.Email));
                        identity.AddClaim(new Claim("access_token", userModel.Token.access_token));
                        identity.AddClaim(new Claim("token_type", userModel.Token.token_type));
                        identity.AddClaim(new Claim("expires_in", userModel.Token.expires_in));
                        identity.AddClaim(new Claim("Email", model.Email));
                        identity.AddClaim(new Claim("Id", Convert.ToString(model.Id)));
                        identity.AddClaim(new Claim("FirstName", model.FirstName));
                        identity.AddClaim(new Claim("LastName", model.LastName));
                        identity.AddClaim(new Claim("Role",Convert.ToString(model.Role))); // set it enum
                        AuthenticationManager.SignOut();
                        AuthenticationManager.SignIn(identity);

                        Global.sharedDataModel.SetSharedData(User);

                        return RedirectToAction("Index", "Profile", new { area = "User" });
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}