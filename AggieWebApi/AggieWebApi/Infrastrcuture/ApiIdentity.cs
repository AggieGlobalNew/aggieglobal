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
using System;
using System.Security.Principal;

namespace AggieGlobal.WebApi.Infrastructure
{
    public class ApiIdentity : IIdentity
    {
        public Account User { get; private set; }

        public ApiIdentity(Account user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            this.User = user;
        }

        public int UserId
        {
            get { return this.User.UserId; }
        }

        public string AuthenticationType
        {
            get { return "Basic"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return this.User.FirstName; }
        }
    }

}