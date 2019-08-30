/* ========================================================================
 * Includes    : FormatterConfig.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Implements various data type formatter registration logics for AggieGlobal.WebApi.
 * Copyright © 2019 Atlas Software Technologies All rights reserved
 * ========================================================================
 * Release History
 * ---------------
 * DESCRIPTION					CREATED / MODIFIED BY
 * -----------                  ---------------------
 * Initial Implementation		Sudipta Sarkar
 * ========================================================================
 */

using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using AggieGlobal.WebApi.Infrastructure;
using AggieGlobal.WebApi.Common;

namespace AggieGlobal.WebApi.Filters
{
    public class AuthorizeAdminAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext != null)
            {
                var query = Utility.GetQueryParameters(actionContext.ControllerContext.Request.RequestUri.Query);

                if (!(query.ContainsKey("key") && query["key"].ToLower().Equals("blindman")))
                {
                    actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                    actionContext.Response.StatusCode = HttpStatusCode.Unauthorized;
                    AggieGlobalLogManager.Warn("Admin Authorization failed");
                }
            }
        }
    }
}