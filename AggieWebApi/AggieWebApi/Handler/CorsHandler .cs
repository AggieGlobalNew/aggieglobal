using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AggieGlobal.WebApi.Controllers.Common;
using System.Net;

namespace AggieGlobal.WebApi.Handler
{
    public class CorsHandler : DelegatingHandler
    {

        private const string origin = "Origin";
        private const string accessControlRequestMethod = "Access-Control-Request-Method";
        private const string accessControlRequestHeaders = "Access-Control-Request-Headers";
        private const string accessControlAllowOrigin = "Access-Control-Allow-Origin";
        private const string accessControlAllowMethods = "Access-Control-Allow-Methods";
        private const string accessControlAllowHeaders = "Access-Control-Allow-Headers";
        private const string accessControlExposeHeaders = "Access-Control-Expose-Headers";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool isPreflightRequest = request.Method == HttpMethod.Options;
            bool isCorsRequest = request.Headers.Contains("Origin");

            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    return Task.Factory.StartNew<HttpResponseMessage>(() =>
                    {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Add("Access-Control-Allow-Origin", request.Headers.GetValues("Origin").First());

                        string accessControlRequestMethod = request.Headers.GetValues("Access-Control-Request-Method").FirstOrDefault();
                        if (accessControlRequestMethod != null)
                        {
                            response.Headers.Add("Access-Control-Allow-Methods", accessControlRequestMethod);
                        }

                        string requestedHeaders = string.Join(", ", request.Headers.GetValues("Access-Control-Request-Headers"));
                        if (!string.IsNullOrEmpty(requestedHeaders))
                        {
                            response.Headers.Add("Access-Control-Allow-Headers", requestedHeaders);
                        }

                        return response;
                    }, cancellationToken);
                }
                else
                {
                    return base.SendAsync(request, cancellationToken).ContinueWith(t =>
                    {
                        var resp = t.Result;
                        resp.Headers.Add(accessControlAllowOrigin, request.Headers.GetValues(origin).First());
                        if (request.Headers.Contains(accessControlExposeHeaders))
                            resp.Headers.Add(accessControlExposeHeaders, request.Headers.GetValues(accessControlExposeHeaders));
                        return resp;
                    });
                }
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}