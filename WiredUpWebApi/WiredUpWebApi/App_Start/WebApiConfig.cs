using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WiredUpWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ConnectionsApi",
                routeTemplate: "api/connections/{action}/{id}",
                defaults: new
                {
                    controller = "connections",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "ConnectionRequestsApi",
                routeTemplate: "api/connectionRequests/{action}/{id}",
                defaults: new
                {
                    controller = "connectionRequests",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "CountriesApi",
                routeTemplate: "api/countries/{action}/{id}",
                defaults: new
                {
                    controller = "countries",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "SkillsApi",
                routeTemplate: "api/skills/{action}/{id}",
                defaults: new
                {
                    controller = "skills",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "CertificatesApi",
                routeTemplate: "api/certificates/{action}/{id}",
                defaults: new
                {
                    controller = "certificates",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "ProjectsApi",
                routeTemplate: "api/projects/{action}/{id}",
                defaults: new
                {
                    controller = "projects",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "UserPostsApi",
                routeTemplate: "api/userposts/{action}/{id}",
                defaults: new
                {
                    controller = "userposts",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "MessagesApi",
                routeTemplate: "api/messages/{action}/{id}",
                defaults: new
                {
                    controller = "messages",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "UsersApi",
                routeTemplate: "api/users/{action}",
                defaults: new
                {
                    controller = "users"
                }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                }
            );

            config.EnsureInitialized();
        }
    }
}
