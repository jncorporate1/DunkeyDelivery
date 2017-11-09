using Microsoft.Owin;
using Owin;
using Stripe;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(DunkeyDelivery.Startup))]
namespace DunkeyDelivery
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["stripeSecretKey"]);
        }
    }
}
