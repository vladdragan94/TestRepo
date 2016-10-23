using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieDictionary.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace MovieDictionary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
