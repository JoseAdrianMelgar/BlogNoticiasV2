using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BlogNoticiasV2.Startup))]

namespace BlogNoticiasV2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
