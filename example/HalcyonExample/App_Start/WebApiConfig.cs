using Halcyon.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HalcyonExample {
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Formatters.RemoveAt(0);
            config.Formatters.Insert(0, new JsonHALMediaTypeFormatter(
                halJsonMedaiTypes: new string[] { "application/hal+json", "application/vnd.example.hal+json", "application/vnd.example.hal.v1+json" },
                jsonMedaiTypes: new string[] { "application/vnd.example+json", "application/vnd.example.v1+json" }
            ));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
