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

using System;
using System.Data.Common;
using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Models.Client;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface IAccountRepository
    {
        bool CreateAccount(Account userData, out bool IsDuplicate);
        string SignIn(string username, string password, string userDeviceId);
        Account GetAcountUserDetailsById(string Email, int userId, string DeviceId);
        AccountResponse GetAcountByUserClientData(string Email, int userId, string DeviceId);
        int UpdateSession(string Email, int userId, string DeviceId);
        int GetAcountUserIdByEmail(string Email, string DeviceId);
    }
}
