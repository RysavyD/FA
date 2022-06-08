using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_3F.Web.Startup))]
namespace _3F.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}