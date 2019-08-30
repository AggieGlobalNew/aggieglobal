using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AggieGlobal.WebApi.Controllers.Common;

namespace AggieGlobal.WebApi.Handler
{
    public class ResponseHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = base.SendAsync(request, cancellationToken);
            // RTM changes 200 to 204 status in case of no content.
            // until clients learn this and adapt we force the status to 200 for all success
            response.Result.StatusCode = response.Result.IsSuccessStatusCode ? System.Net.HttpStatusCode.OK : response.Result.StatusCode;

            foreach (var property in request.Properties.Where(p => p.Key.ToString().StartsWith(WebHeaders.Prefix)))
            {
                var key = property.Key.Replace(WebHeaders.Prefix, string.Empty);
                if (key.StartsWith("Content"))
                {
                    //response.Result.Content.Headers.Add(key, property.Value.ToString() + " ");
                    HttpContext.Current.Response.AppendHeader(key, property.Value.ToString());
                }
                else
                {
                    response.Result.Headers.Add(key, property.Value.ToString());
                }
            }

            return response;
        }
    }    
}