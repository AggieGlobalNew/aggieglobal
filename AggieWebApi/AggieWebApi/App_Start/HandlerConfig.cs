/* ========================================================================
 * Includes    : HandlerConfig.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Implements various handler registration logics for AggieGlobal.WebApi.
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

using System.Collections.ObjectModel;
using System.Net.Http;
using AggieGlobal.WebApi.Handler;

namespace AggieGlobal.WebApi
{
    public class HandlerConfig
    {
        public static void RegisterHandlers(Collection<DelegatingHandler> handlers)
        {
            handlers.Add(new CorsHandler());
            handlers.Add(new ResponseHandler());
        }
    }
}