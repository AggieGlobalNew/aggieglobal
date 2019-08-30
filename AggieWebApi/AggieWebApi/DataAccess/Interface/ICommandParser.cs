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
using System.Data.Common;

namespace AggieGlobal.WebApi.DataAccess.Interface
{
    public interface ICommandParser
    {
        DbCommand GetCommand<TItem>(DbConnection connection, Method method, params object[] values);
        DbCommand GetCommand(string key, DbConnection connection, Method method, params object[] values);
        DbCommand GetCommand(string query, DbConnection connection);
        string GetCommand(string key, Method method, params object[] values);
        DbCommand PrepareCommand<TItem>(DbConnection connection, Method method, string caption, params object[] values);
        DbCommand PrepareCommand(string key, DbConnection connection, Method method, string caption, params object[] values);
        string PrepareCommand(string key, Method method, string caption, params object[] values);
    }
}