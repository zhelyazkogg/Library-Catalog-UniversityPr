using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LibCatalog.Startup))]
namespace LibCatalog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
