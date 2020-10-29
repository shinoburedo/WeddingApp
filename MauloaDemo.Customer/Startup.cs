using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MauloaDemo.Customer.Startup))]
namespace MauloaDemo.Customer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
