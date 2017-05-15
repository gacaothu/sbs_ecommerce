using Microsoft.Owin;
using Owin;
using SBS_Ecommerce.Models.Extension;

[assembly: OwinStartup(typeof(SBS_Ecommerce.Startup))]
namespace SBS_Ecommerce
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AutoMapperConfiguration.Instance.Configure();
            ConfigureAuth(app);
        }
    }
}
