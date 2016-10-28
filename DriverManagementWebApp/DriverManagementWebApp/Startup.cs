using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DriverManagementWebApp.Startup))]
namespace DriverManagementWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
