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

                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Email == context.UserName && x.Password == context.Password);
                    if (user != null)
                    {
                        identity.AddClaim(new Claim("username", context.UserName));
                        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                        switch (user.Role)
                        {
                            case 0:
                                identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.User));
                                break;
                            //case 1:
                            //    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Deliverer));
                            //    break;
                            case 2:
                                identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SubAdmin));
                                break;
                            case 3:
                                identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SuperAdmin));
                                break;
                            case 4:
                                identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.ApplicationAdmin));
                                break;
                            default:
                                break;
                        }
                        //identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.User));
                        context.Validated(identity);
                    }
                    else if (context.UserName == "admin" && context.Password == "admin")
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                        identity.AddClaim(new Claim("username", "admin"));
                        identity.AddClaim(new Claim(ClaimTypes.Name, "admin"));
                        context.Validated(identity);
                    }
                    else
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(ctx.Users.FirstOrDefault());
                        context.SetError("invalid username or password!", json);
                        //context.Response.Headers.Add("AuthorizationResponse", new[] { "Failed" });
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