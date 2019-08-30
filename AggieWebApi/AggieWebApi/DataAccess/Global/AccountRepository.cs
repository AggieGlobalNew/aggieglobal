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
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Infrastructure;
using AggieGlobal.WebApi.Models.Client;

namespace AggieGlobal.WebApi.DataAccess
{
    internal class AccountRepository : SqlDataAccessRepository<Account>, IAccountRepository
    {
        public AccountRepository()
        {
        }

        public AccountRepository(DbTransaction trans)
            : base(trans)
        {
        }

        public bool CreateAccount(Account userData, out bool IsDuplicate)
        {
            try
            {
                DbTransaction transaction = null;
                int result = default(int);
                IsDuplicate = false;
                using (var connection = GetConnection())
                {
                    try
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();
                        userData.optMode = 2;
                        AggieGlobalLogManager.Info("RegistrationController :: Registration started  Account Repository");
                        CreateRecordWithinTransaction("CreateAccount", transaction, userData.FirstName, userData.LastName,userData.password,userData.Address,userData.EmailId,userData.FarmId,userData.UserDeviceId, userData.optMode,userData.AuthenticationSuccessmode);
                        result = 1;
                        transaction.Commit();
                        AggieGlobalLogManager.Info("RegistrationController :: Registration ended  Account Repository");

                        if (result==1)
                        {
                            int userNewId = GetAcountUserIdByEmail(userData.EmailId, userData.UserDeviceId);
                            userData.UserId = userNewId;
                            userData.AuthToken = EncryptionHelper.AesEncryption(userData.EmailId + "-" + userNewId + "-" + userData.UserDeviceId,EncryptionKey.LOG);
                            return (result == 1) ? true : false;
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = string.Empty;
                        using (System.IO.StringReader reader = new System.IO.StringReader(ex.Message))
                        {
                            error = reader.ReadLine();
                        }
                        string[] errorObj = null;
                        if (!string.IsNullOrEmpty(error))
                        errorObj=error.Split(Environment.NewLine.ToCharArray());
                        if(errorObj[0]=="1"? IsDuplicate=true:false)
                        result = 0;
                        AggieGlobalLogManager.Fatal("RegistrationController :: Register failed :: " + ex.Message);
                        return Convert.ToBoolean(userData.AuthenticationSuccessmode);
                    }
                    return Convert.ToBoolean(result);
                }
            }
            catch (Exception e)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: CreateAccount failed :: " + e.Message);
            }
            IsDuplicate = false;
            return Convert.ToBoolean(userData.AuthenticationSuccessmode);
        }

        public string SignIn(string username, string password,string userDeviceId)
        {
            bool res = default(bool);
            IEnumerable<Account> userData = null;
            int OpMode = default(int);
            string LoginTokenKey = string.Empty;
            int AuthenticationSuccessmode = default(int);
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    userData = GetRecord("GetAccountDetails", username, password, userDeviceId, AuthenticationSuccessmode);
                }
                if (userData != null && userData.Count() > default(int) && userData.FirstOrDefault().UserId>default(int))
                {
                    LoginTokenKey = username + "-" + userData.FirstOrDefault().UserId + "-" + userDeviceId;
                    return EncryptionHelper.AesEncryption(LoginTokenKey, EncryptionKey.LOG);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: LoginCheck failed :: " + ex.Message);
            }
            return string.Empty;
        }

        public Account GetAcountUserDetailsById(string Email, int userId, string DeviceId)
        {
            bool res = default(bool);
            IEnumerable<Account> userData = null;
            int OpMode = default(int);
            string LoginTokenKey = string.Empty;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    userData = GetRecord("GetAccountUserDetails", Email, userId);
                    return userData.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: GetAcountByUserId failed :: " + ex.Message);
            }
            return null;
        }
        public int GetAcountUserIdByEmail(string Email, string DeviceId)
        {
            bool res = default(bool);
            IEnumerable<Account> userData = null;
            int OpMode = default(int);
            string LoginTokenKey = string.Empty;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    userData = GetRecord("GetAcountUserIdByEmail", Email,DeviceId);
                    return userData.FirstOrDefault().UserId;
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: GetAcountUserIdByEmail failed :: " + ex.Message);
            }
            return default(int);
        }

        public AccountResponse GetAcountByUserClientData(string Email, int userId, string DeviceId)
        {
            bool res = default(bool);
            IEnumerable<Account> userData = null;
            int OpMode = default(int);
            string LoginTokenKey = string.Empty;
            AccountResponse iresponse = null;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    userData = GetRecord("GetAcountByUserClientData", Email, userId);
                    if (userData != null && userData.Count() > default(int))
                    {
                        iresponse = new AccountResponse();
                        iresponse.EmailId = userData.FirstOrDefault().EmailId;
                        iresponse.FirstName = userData.FirstOrDefault().FirstName;
                        iresponse.LastName = userData.FirstOrDefault().LastName;
                        iresponse.Address = userData.FirstOrDefault().Address;
                        iresponse.IsAdmin = userData.FirstOrDefault().IsAdmin;
                    }

                    return iresponse;
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: GetAcountByUserId failed :: " + ex.Message);
            }
            return null;
        }

        public int UpdateSession(string Email, int userId, string DeviceId)
        {
            int res = default(int);
            int OpMode = default(int);
            string LoginTokenKey = string.Empty;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    res = UpdateRecord("UpdateAccountDetails", Email, userId, DeviceId);
                    return res;
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: UpdateSession failed :: " + ex.Message);
            }
            return res;
        }
    }
}