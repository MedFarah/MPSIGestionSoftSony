using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MPSIGestionSoftSony.Startup))]
namespace MPSIGestionSoftSony
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}