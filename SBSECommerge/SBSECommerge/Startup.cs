using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SBSECommerge.Startup))]
namespace SBSECommerge
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
