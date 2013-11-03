using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace WiredUpWebApi.Tests
{
    internal class InMemoryHttpServer
    {
        private readonly HttpClient client;
        private readonly string baseUrl;

        public InMemoryHttpServer(string baseUrl)
        {
            this.baseUrl = baseUrl;
            var config = new HttpConfiguration();
            this.AddHttpRoutes(config.Routes);
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            var server = new HttpServer(config);
            this.client = new HttpClient(server);
        }

        public HttpResponseMessage CreateGetRequest(string requestUrl, string mediaType = "application/json")
        {
            var url = requestUrl;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = HttpMethod.Get;

            var response = this.client.SendAsync(request).Result;
            return response;
        }

        public HttpResponseMessage CreatePostRequest(string requestUrl, object data, string mediaType = "application/json")
        {
            var url = requestUrl;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = HttpMethod.Post;
            request.Content = new StringContent(JsonConvert.SerializeObject(data));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            var response = this.client.SendAsync(request).Result;
            return response;
        }

        public HttpResponseMessage CreatePutRequest(string requestUrl, object data, string mediaType = "application/json")
        {
            var url = requestUrl;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = HttpMethod.Put;
            request.Content = new StringContent(JsonConvert.SerializeObject(data));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            var response = this.client.SendAsync(request).Result;
            return response;
        }

        public HttpResponseMessage CreateDeleteRequest(string requestUrl, string mediaType = "application/json")
        {
            var url = requestUrl;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = HttpMethod.Delete;

            var response = this.client.SendAsync(request).Result;
            return response;
        }

        private void AddHttpRoutes(HttpRouteCollection routeCollection)
        {
            var routes = GetRoutes();
            routes.ForEach(route => routeCollection.MapHttpRoute(route.Name, route.Template, route.Defaults));
        }

        private List<Route> GetRoutes()
        {
            return new List<Route>
            {
                 new Route(
                    "CertificatesApi",
                    "api/certificates/{action}/{id}",
                    new
                    {
                        controller = "certificates",
                        id = RouteParameter.Optional
                    }),

                new Route(
                    "ProjectsApi",
                    "api/projects/{action}/{id}",
                    new
                    {
                        controller = "projects",
                        id = RouteParameter.Optional
                    }),

                new Route(
                    "UserPostsApi",
                    "api/userposts/{action}/{id}",
                    new
                    {
                        controller = "userposts",
                        id = RouteParameter.Optional
                    }),

                new Route(
                    "MessagesApi",
                    "api/messages/{action}/{id}",
                    new
                    {
                        controller = "messages",
                        id = RouteParameter.Optional
                    }),

                new Route(
                    "UsersApi",
                    "api/users/{action}",
                    new
                    {
                        controller = "users"
                    }),

                new Route(
                    "DefaultApi",
                    "api/{controller}/{id}",
                    new
                    {
                        id = RouteParameter.Optional
                    })
            };
        }

        private class Route
        {
            public Route(string name, string template, object defaults)
            {
                this.Name = name;
                this.Template = template;
                this.Defaults = defaults;
            }

            public object Defaults { get; set; }

            public string Name { get; set; }

            public string Template { get; set; }
        }
    }
}
