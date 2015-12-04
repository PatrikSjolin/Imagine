using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Imagine.Startup))]
namespace Imagine
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
