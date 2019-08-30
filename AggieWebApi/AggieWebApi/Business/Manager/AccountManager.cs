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
using AggieGlobal.WebApi.Business;
using AggieGlobal.WebApi.Business.Managers;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieWebApi.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.Business.Manager
{
    internal class AccountManager : ManagerBase, IAccountManager
    {
        #region Member Variables


        private readonly IGlobalApp _globalApp = null;

        protected Account CurrentUser { get; set; }
        #endregion



        #region Constructor
        public AccountManager(string dbConnectionStringName)
            : base(dbConnectionStringName)
        {
            this._globalApp = GlobalApp.Instance;
            //this.CurrentUser = userIdentity;
        }
        #endregion

        public bool CreateAccount(Account userdata,out bool IsDuplicate)
        {
            bool result = default(bool);
            IsDuplicate = false;
            try
            {
                result = new RepositoryCreator().AccountRepository.CreateAccount(userdata,out IsDuplicate);

                if(result==true && userdata.UserId>default(int))
                {
                    var connectionString = "AggieGlobal";
                    FarmManager fmanager = new FarmManager(connectionString);
                    FarmDetail fDetail = new FarmDetail();
                    fDetail.FarmId = default(int);
                    fDetail.FarmName = "Farm_" +  userdata.EmailId;
                    fDetail.FarmSize = default(int);
                    fDetail.FarmSizeUnit = string.Empty;
                    fDetail.FarmAddress = string.Empty;
                    fDetail.FarmEstablishedDate = DateTime.Now;
                    userdata.FarmId = fmanager.CreateUpdateFarm(fDetail);
                    if(userdata.FarmId>default(int))
                    {
                        fmanager.MapFarmByUserDetail(userdata.FarmId,userdata.UserId);
                    }
                    fmanager.Dispose();
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountManager :: CreateAccount failed :: " + ex.Message);
            }
            return result;
        }

        public string LoginCheck(string username, string password, string userDeviceId)
        {
            bool res = default(bool);
            try
            {
                return new RepositoryCreator().AccountRepository.SignIn(username,password,userDeviceId);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountManager :: LoginCheck failed :: " + ex.Message);
            }
            return string.Empty;
        }

        public Account GetAcountByUserId(string Email, int userId, string DeviceId)
        {
            Account user = null;
            try
            {
                user = new RepositoryCreator().AccountRepository.GetAcountUserDetailsById(Email,userId,DeviceId);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountManager :: CreateAccount failed :: " + ex.Message);
            }
            return user;
        }

        public AccountResponse GetAcountByUserClientData(string Email, int userId, string DeviceId)
        {
            AccountResponse user = null;
            try
            {
                user = new RepositoryCreator().AccountRepository.GetAcountByUserClientData(Email, userId, DeviceId);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountManager :: CreateAccount failed :: " + ex.Message);
            }
            return user;
        }

        public int UpdateSession(string Email, int userId, string DeviceId)
        {
            int res = default(int);
            try
            {
                res = new RepositoryCreator().AccountRepository.UpdateSession(Email, userId, DeviceId);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountManager :: CreateAccount failed :: " + ex.Message);
            }
            return res;
        }

        public void Dispose()
        {

        }
    }
}