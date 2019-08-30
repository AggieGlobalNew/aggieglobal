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

using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Models.Client;
using System;

namespace AggieGlobal.WebApi.DataAccess.Global
{
    internal class AuthorizedIPRepository : SqlDataAccessRepository<AuthorizedIP>, IAuthorizedIPRepository
    {
        public AuthorizedIP GetByAccountId(int pwAccountId, int pcModuleId)
        {
            throw new NotImplementedException();
        }
    }
}