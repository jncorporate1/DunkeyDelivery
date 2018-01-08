using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using DAL;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;
using DunkeyDelivery.CustomAuthorization;

namespace DunkeyDelivery
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public static OAuthGrantResourceOwnerCredentialsContext AuthorizeContext;
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                var form = await context.Request.ReadFormAsync();
                var SignInType = 0;
                //var SignInType = Convert.ToInt32(form["signintype"]);

                var Role1 = Convert.ToInt32(form["role"]);
                var Role2= Convert.ToInt32(form["SignInType"]);

                if(Role1 == 0 && Role2 != 0)
                {
                    SignInType = Role2;
                }else if(Role1 != 0 && Role2 == 0)
                {
                    SignInType = Role1;
                }else
                {
                    SignInType = Role1;
                }

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    //DeliveryMan deliveryManModel = null;
                    User userModel = null;
                    Admin adminModel = null;

                    if (SignInType == 1)
                    {
                        //deliveryManModel = ctx.DeliveryMen.FirstOrDefault(x => x.Email == context.UserName && x.Password == context.Password);
                        //if (deliveryManModel != null)
                        //{
                        //    identity.AddClaim(new Claim("username", context.UserName));
                        //    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                        //    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Deliverer.ToString()));
                        //    context.Validated(identity);
                        //}
                        //else
                        //{
                        //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(ctx.Users.FirstOrDefault());
                        //    context.SetError("invalid username or password!", json);
                        //}
                    }
                    else if (SignInType == 0 || SignInType == 5)
                    {
                        if (SignInType == 5)
                        {
                            userModel = ctx.Users.FirstOrDefault(x => x.Email == context.UserName && x.Role == 5 && x.IsDeleted == false);
                        }
                        else
                            userModel = ctx.Users.FirstOrDefault(x => x.Email == context.UserName && x.Password == context.Password);

                        if (userModel != null)
                        {
                            identity.AddClaim(new Claim("username", context.UserName));
                            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                            switch (SignInType)
                            {
                                case 0:
                                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.User.ToString()));
                                    break;
                                //case 1:
                                //    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Deliverer.ToString()));
                                //    break;
                                case 2:
                                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SubAdmin.ToString()));
                                    break;
                                case 3:
                                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SuperAdmin.ToString()));
                                    break;
                                case 4:
                                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.ApplicationAdmin.ToString()));
                                    break;
                                case 5:
                                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Facebook.ToString()));
                                    break;
                                default:
                                    break;
                            }
                            context.Validated(identity);
                        }
                        else
                        {
                            var json = Newtonsoft.Json.JsonConvert.SerializeObject(ctx.Users.FirstOrDefault());
                            context.SetError("invalid username or password!", json);
                        }
                    }
                    else if (SignInType == 2 || SignInType == 3 || SignInType == 4)
                    {
                        adminModel = ctx.Admins.FirstOrDefault(x => x.Email == context.UserName && x.Password == context.Password);
                        if (adminModel != null)
                        {
                            identity.AddClaim(new Claim("username", context.UserName));
                            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                            switch (adminModel.Role)
                            {
                                case 0:
                                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.User.ToString()));
                                    break;
                                //case 1:
                                //    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Deliverer.ToString()));
                                //    break;
                                case 2:
                                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SubAdmin.ToString()));
                                    break;
                                case 3:
                                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SuperAdmin.ToString()));
                                    break;
                                case 4:
                                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.ApplicationAdmin.ToString()));
                                    break;
                                default:
                                    break;
                            }
                            context.Validated(identity);
                        }
                        else
                        {
                            var json = Newtonsoft.Json.JsonConvert.SerializeObject(ctx.Users.FirstOrDefault());
                            context.SetError("invalid username or password!", json);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            #region commented
            //if (context.UserName == "admin" && context.Password == "admin")
            //{
            //    //identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
            //    identity.AddClaim(new Claim("username", "admin"));
            //    identity.AddClaim(new Claim(ClaimTypes.Name, "admin"));
            //    context.Validated(identity);
            //}
            //else if (context.UserName == "user" && context.Password == "user")
            //{
            //    //identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            //    identity.AddClaim(new Claim("username", "user"));
            //    identity.AddClaim(new Claim(ClaimTypes.Name, "Mohsin"));
            //    context.Validated(identity);
            //} 
            #endregion
        }

        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

    }
}