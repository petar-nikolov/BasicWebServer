using HttpWebServer.Common;
using HttpWebServer.HTTP;
using HttpWebServer.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpWebServer.Routing
{
    public class RoutingTable : IRoutingTable
    {
        private readonly IDictionary<Method, IDictionary<string, Response>> _routes;

        public RoutingTable() =>
        _routes = new Dictionary<Method, IDictionary<string, Response>>
        {
            [Method.Get] = new Dictionary<string, Response>(),
            [Method.Post] = new Dictionary<string, Response>(),
            [Method.Put] = new Dictionary<string, Response>(),
            [Method.Delete] = new Dictionary<string, Response>()
        };

        public IRoutingTable Map(string url, Method method, Response response) =>
        method switch
        {
            Method.Get => MapGet(url, response),
            Method.Post => MapPost(url, response),
            _ => throw new InvalidOperationException($"Method '{method}' is not supported.")
        };

        public IRoutingTable MapGet(string url, Response response)
        {
            Guard.AgainstNull(url, nameof(url));
            Guard.AgainstNull(response, nameof(response));

            _routes[Method.Get][url] = response;
            return this;
        }

        public IRoutingTable MapPost(string url, Response response)
        {
            Guard.AgainstNull(url, nameof(url));
            Guard.AgainstNull(response, nameof(response));

            _routes[Method.Post][url] = response;
            return this;
        }

        public Response MatchRequest(Request request)
        {
            var requestMethod = request.Method;
            var requestUrl = request.Url;
            if (!_routes.ContainsKey(requestMethod) ||
                !_routes[requestMethod].ContainsKey(requestUrl))
            {
                return new NotFoundResponse();
            }

            return _routes[requestMethod][requestUrl];
        }
    }
}
