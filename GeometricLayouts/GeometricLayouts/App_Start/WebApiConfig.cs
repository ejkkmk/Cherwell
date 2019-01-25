using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace GeometricLayouts
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "GetCoordinatesRowCol",
                routeTemplate: "api/{controller}/{RowString}/{ColString}"
            );

            config.Routes.MapHttpRoute(
                name: "GetRowCol",
                routeTemplate: "api/{controller}/{Pt1x}/{Pt1y}/{Pt2x}/{Pt2y}/{Pt3x}/{Pt3y}/"
);

        }
    }
}
