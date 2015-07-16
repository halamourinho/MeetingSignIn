using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MeetingSignIn
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}"
            );
        }

        public static void Main()
        {
            var v = new {s = "111", x = 111};
            Console.WriteLine(v.GetType());
        }
    }
}
