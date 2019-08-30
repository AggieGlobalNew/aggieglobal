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
