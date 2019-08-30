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

using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public abstract class BaseDataAccessRepository
    {
        protected string _connectionStringName = null;

        protected BaseDataAccessRepository(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
        }

        protected DbConnection GetConnection()
        {
            return DatabaseFactory.CreateDatabase(_connectionStringName).CreateConnection();
        }
    }
}