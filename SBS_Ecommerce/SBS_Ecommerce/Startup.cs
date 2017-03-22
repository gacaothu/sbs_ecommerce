using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SBS_Ecommerce.Startup))]
namespace SBS_Ecommerce
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
