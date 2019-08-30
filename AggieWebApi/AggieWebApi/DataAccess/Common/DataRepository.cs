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

namespace AggieGlobal.WebApi.DataAccess.Common
{
    internal class DataRepository : SqlDataAccessRepository<dynamic>
    {
        internal DataRepository(string connectionString)
            : base(connectionString)
        {}

        public IEnumerable<dynamic> GetQueryResult(string query)
        {
            //return GetData(query);
            return new List<dynamic>();
        }

        public int ExecuteQuery(string query)
        {
            //return Execute(query);
            return 0;
        }
    }
}