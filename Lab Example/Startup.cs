using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lab_Example.Startup))]
namespace Lab_Example
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
