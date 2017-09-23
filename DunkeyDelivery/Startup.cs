using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DunkeyDelivery.Startup))]
namespace DunkeyDelivery
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
