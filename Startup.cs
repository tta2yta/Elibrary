using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ELibrary.Startup))]
namespace ELibrary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
