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


using System.Collections.Generic;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Models.Public;

namespace AggieGlobal.WebApi.DataAccess
{
    internal class CodesRepository : SqlDataAccessRepository<LK_Codes>, ICodesRepository
    {
        public IEnumerable<LK_Codes> GetByCodeIdentifier(string codeIdentifier)
        {
            return GetRecord("GetByCodeIdentifier", codeIdentifier);
        }
    }
}