using System.Web.Http;

namespace Triggerless.API

{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
              name: "RipLog",
              routeTemplate: "api/riplog/summary",
              defaults: new { controller = "riplog", action = "RipLogSummary" }
            );

            config.Routes.MapHttpRoute(
              name: "RipLogByIpExt",
              routeTemplate: "api/riplog/ipx/{a1}.{a2}.{a3}.{a4}/",
              defaults: new { controller = "riplog", action = "RipLogByIpExt" }
            );

            config.Routes.MapHttpRoute(
              name: "RipLogByIp",
              routeTemplate: "api/riplog/ip/{a1}.{a2}.{a3}.{a4}/",
              defaults: new { controller = "riplog", action = "RipLogByIp" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.EnableCors();
        }
    }
}
