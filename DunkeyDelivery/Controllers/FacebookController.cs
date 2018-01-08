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
    public class FacebookController : Controller
    {

        private Uri RediredtUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        // GET: Facebook
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["Client_Id"],
                client_secret = ConfigurationManager.AppSettings["Client_Secret"],
                redirect_uri = RediredtUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> FacebookCallback(string code)
        {
            try
            {
                var fb = new FacebookClient();
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = ConfigurationManager.AppSettings["Client_Id"],
                    client_secret = ConfigurationManager.AppSettings["Client_Secret"],
                    redirect_uri = RediredtUri.AbsoluteUri,
                    code = code
                });

                var accessToken = result.access_token;
                Session["AccessToken"] = accessToken;
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");

                string email = me.email;
                TempData["email"] = me.email;
                TempData["first_name"] = me.first_name;
                TempData["lastname"] = me.last_name;
                TempData["picture"] = me.picture.data.url;
                LoginViewModel model = new LoginViewModel();
                model.Email = me.email;
                model.Password = string.Empty;
                model.FirstName = me.first_name;
                model.LastName = me.last_name;
                model.ProfilePictureUrl = me.picture.data.url;



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
                    if (userModel.Role == Convert.ToInt32(Utility.RoleTypes.Facebook))
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
                        identity.AddClaim(new Claim(ClaimTypes.Role, "Facebook")); // set it enum
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