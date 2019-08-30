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

using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Controllers.Common;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AggieWebApi.Controllers
{
    public class LoginController<Item> : AbstractController<Account>
    {
        public LoginController()
        {
            var jsonSchemaGenerator = new JsonSchemaGenerator();
            var myType = typeof(ActivityDetailResponse);
            var schema = jsonSchemaGenerator.Generate(myType);
            schema.Title = myType.Name;
        }

        [ActionName("Signin")]
        public bool Signin()
        {
            bool ret = default(bool);

            return ret;
        }
        
    }
}
