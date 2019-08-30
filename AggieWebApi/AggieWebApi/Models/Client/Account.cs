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

using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.Models.Client
{
    public class Account : ModelBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserTypeId { get; set; }
        public string password { get; set; }
        public string Address { get; set; }
        public int UserId { get; set; }
        public bool IsExpired { get; set; }
        public AuthReason AuthReason { get; set; }
        public string EmailId { get; set; }
        public int FarmId { get; set; }
        public int optMode { get; set; }
        public string UserDeviceId { get; set; }
        public string LoginTokenKey { get; set; }
        public int AuthenticationSuccessmode { get; set; }
        public int AuthenticatedUserID { get; set; }
        public bool IsAdmin { get; set; }
        public string AuthToken { get; set; }
    }
    public class AccountResponse : ModelBase
    {
        public string AuthToken { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string EmailId { get; set; }
        public string FarmId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class CommonModuleResponse : ModelBase
    {
        public byte[] fileStream { get; set;}
        public ProductDetailResponse productdata { get; set; }
    }
}
