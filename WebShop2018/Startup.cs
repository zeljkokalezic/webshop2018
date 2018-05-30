using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebShop2018.Startup))]
namespace WebShop2018
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
