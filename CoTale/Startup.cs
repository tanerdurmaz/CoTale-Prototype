using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CoTale.Startup))]
namespace CoTale
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
