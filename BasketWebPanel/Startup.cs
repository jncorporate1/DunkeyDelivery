using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BasketWebPanel.Startup))]
namespace BasketWebPanel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
