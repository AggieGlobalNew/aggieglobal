/* ========================================================================
 * Includes    : RequestContext.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Implements logics for handling incoming HTTP requests for AggieGlobal.WebApi.
 * Copyright © 2019 Atlas Software Technologies All rights reserved
 * ========================================================================
 * Release History
 * ---------------
 * DESCRIPTION					CREATED / MODIFIED BY
 * -----------                  ---------------------
 * Initial Implementation		Sudipta Sarkar
 * ========================================================================
 * Change History
 * --------------
 */

using System.Linq;
using System.Web;
using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Infrastructure;

namespace AggieGlobal.WebApi.Controllers.Common
{
    public interface IRequestContext
    {
        string GetHeaderValue(string header);
        Account CurrentUser { get; }
        bool ContainsHeader(string headerKey);
    }

    internal class WebRequestContext : IRequestContext
    {
        public string GetHeaderValue(string headerKey)
        {
            return HttpContext.Current.Request.Headers.AllKeys.Contains(headerKey) ?
                HttpContext.Current.Request.Headers[headerKey] : string.Empty;
        }

        public Account CurrentUser
        {
            get { return ((ApiIdentity)HttpContext.Current.User.Identity).User; }
        }

        public bool ContainsHeader(string headerKey)
        {
            return HttpContext.Current.Request.Headers.AllKeys.Contains(headerKey);
        }
    }
}
