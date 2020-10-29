using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;
using Newtonsoft.Json.Serialization;

namespace MauloaDemo.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //属性ベースのRoutingを有効にした場合はこちらを使う。
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //WebApiConfig.Register(GlobalConfiguration.Configuration);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            App_Start.BootstrapBundleConfig.RegisterBundles();

            //Remove XML Format for APIs so JSON will be the default format.
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            // Tell Json serializer to ignore [Serializable] attribute.
            var serializerSettings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            var contractResolver = (DefaultContractResolver)serializerSettings.ContractResolver;
            contractResolver.IgnoreSerializableAttribute = true;

            
            //ViewEngines.Engines.Clear();
            //ViewEngines.Engines.Add(new RazorViewEngine());

            //DisplayModeProvider.Instance.Modes.Clear();
            //DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode());
            //DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("Mobile") {
            //    ContextCondition = (cntxt => 
            //            //iPadは常にPCとして扱う。それ以外はデフォルトのIsMobileDevice判定に委ねる。
            //            cntxt.GetOverriddenUserAgent().IndexOf("iPad", StringComparison.OrdinalIgnoreCase) >= 0 ? false 
            //                : cntxt.GetOverriddenBrowser().IsMobileDevice)
            //});

        }
    }
}