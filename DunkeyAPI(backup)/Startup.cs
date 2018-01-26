using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;
using AutoMapper;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;

[assembly: OwinStartup(typeof(DunkeyDelivery.Startup))]

namespace DunkeyDelivery
{
    public partial class Startup
    {
        public object Httpconfiguration { get; private set; }
        public static OAuthAuthorizationServerOptions OAuthServerOptions;
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions;
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            //app.Use<CustomAuthenticationMiddleware>(); //Use this middleware to return custom error codes instead of 400 in GrantResourceOwnerCredentials.
            ConfigureOAuth(app);

            HttpConfiguration config = new HttpConfiguration();
            app.UseWebApi(config);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DAL.Product, DunkeyAPI.ViewModels.MedicationNames>();
                cfg.CreateMap<DAL.Category, DunkeyAPI.ViewModels.ParentCategory>();
                cfg.CreateMap<DAL.Category, DunkeyAPI.ViewModels.categoryViewModel>();
                cfg.CreateMap<DAL.Product, DunkeyAPI.ViewModels.productslist>();
                cfg.CreateMap<DAL.Category, DunkeyAPI.ViewModels.SubCategories>();
                cfg.CreateMap<DAL.UserAddress, DunkeyAPI.ViewModels.AddressViewModel>();
                cfg.CreateMap<DAL.CreditCard, DunkeyAPI.ViewModels.CreditCards>();
                cfg.CreateMap<DAL.BlogPosts, DunkeyAPI.ViewModels.BlogViewModel>();
                cfg.CreateMap<DAL.Store, DunkeyAPI.ViewModels.AlcoholViewModel>();
            });
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AuthenticationType = OAuthDefaults.AuthenticationType,
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                Provider = new MyAuthorizationServerProvider(),
                //RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };
            //var facebookAuthenticationOptions = new FacebookAuthenticationOptions()
            //{
            //    AppId = "1535577566495675",
            //    AppSecret = "fa855ddd9813fb417b4c44e74e24911f",
            //    UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name,location",
            //    Scope = { "email" }
            //};

            //app.UseFacebookAuthentication(facebookAuthenticationOptions);
            // Token Generation
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "286033043421-13bp1i0ivrlq84i9agh3uk2nma90fs7k.apps.googleusercontent.com",
            //    ClientSecret = "HFS7L1d65Oc4arIvn9JcHf3P",
            //    CallbackPath = new PathString("/Home")
            //});

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "279270651973-o98bchtvhdvacm66ukf137usfjkogr43.apps.googleusercontent.com",
            //    ClientSecret = "C7Puvd3YM2F6IQxqSOEAbm7F"
            //});

        }



    }
}
