using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using MauloaDemo.Models;

namespace MauloaDemo.Customer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            //属性ベースのRoutingを有効にする。
            config.MapHttpAttributeRoutes();

            //ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            //builder.EntitySet<Customer>("CustomersOData");
            //config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //全てのAPIに認証を要求する。
            config.Filters.Add(new AuthorizeAttribute());
        }
    }
}
