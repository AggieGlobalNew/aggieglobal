/* ========================================================================
 * Includes    : ResponseContext.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Implements logics for handling outgoing HTTP response from AggieGlobal.WebApi.
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

using System.Collections.Generic;

namespace AggieGlobal.WebApi.Controllers.Common
{
    public interface IResponseContext
    {
        void SetProperties(IDictionary<string, object> properties, string headerKey, string headerValue);
    }

    internal class WebResponseContext : IResponseContext
    {
        public void SetProperties(IDictionary<string, object> properties, string headerKey, string headerValue)
        {
            string strHeaderKey = "Header." + headerKey;
            if (properties.ContainsKey(strHeaderKey))
            {
                properties[strHeaderKey] = headerValue;
            }
            else
            {
                properties.Add(strHeaderKey, headerValue);
            }
        }
    }
}