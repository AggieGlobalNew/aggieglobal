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

using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace AggieGlobal.WebApi.Filters
{
    public sealed class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception != null && !(context.Exception is WebApiException))
            {
                AggieGlobalLogManager.Fatal(context.Exception, "Exception handled in MVC-Controller, Request URI:{0}", context.Request.RequestUri);
            }
            else
            {
                AggieGlobalLogManager.Warn(context.Exception, "Exception handled in MVC-Controller, Request URI:{0}", context.Request.RequestUri);
            }

            var response = new HttpResponseMessage(); // { ReasonPhrase = context.Exception == null ? string.Empty : context.Exception.Message };
            // No nead to write reason phrase as no one needs it now
            var exception = context.Exception as WebApiException;
            response.StatusCode = exception != null ? exception.Status : HttpStatusCode.InternalServerError;
           //if(exception != null && exception.Error== ErrorList.PasswordExpired)
           //    response.Content = new ErrorObject(exception != null ? exception.Error : ErrorList.UnknownException, exception.Message).ToContent();
           //else
            response.Content = new ErrorObject(exception != null ? exception.Error : ErrorList.UnknownException).ToContent();
            context.Response = response;
        }
    }
}
