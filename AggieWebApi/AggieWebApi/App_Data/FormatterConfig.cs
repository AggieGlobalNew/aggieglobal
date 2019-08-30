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
 * Change History
 * --------------
 * Date: Tue, Mar 12, 2019 By Sudipta Sarkar
 * -------------------------------------
 * 1.ConvertToClient [Existing]: Implements logic for avoiding datetime conversion without valid user identity.
 */

using System;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Web.Http;
using AggieGlobal.WebApi.DataAccess.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AggieGlobal.WebApi
{
    public class FormatterConfig
    {
        public static void RegisterFormatters(MediaTypeFormatterCollection formatters)
        {
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.Indent = true;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public static JsonMediaTypeFormatter JsonFormatter
        {
            get
            {
                return GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            }
        }
    }


}