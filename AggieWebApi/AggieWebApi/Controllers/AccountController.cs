using AggieGlobal.Business.Manager;
using AggieGlobal.Models.Client;
using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Business;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.Controllers.Common;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Infrastructure;
using AggieWebApi.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AggieWebApi.Controllers
{
    public class AccountController : AbstractController<Account>
    {
        public AccountController()
            : this(null)
        {

        }

        public AccountController(IAccountRepository aRepository)
            : base()
        { }

        [HttpPost]
        [ActionName("Register")]
        public AccountResponse Register([FromBody] Account userData)
        {

            bool res = default(bool);
            AccountResponse ibase = new AccountResponse();
            try
            {

                if (userData == null)
                {
                    ibase.Status = ResponseStatus.Failed;
                    ibase.Error = "Required parameters not set";
                }
                else
                {
                    AggieGlobalLogManager.Info("RegistrationController :: Registration started ");
                    var connectionString = "AggieGlobal";
                    var repo = new AccountManager(connectionString);
                    bool IsDuplicate = false;
                    //res = AccountManager.CreateAccount(userData, out IsDuplicate);
                    res = repo.CreateAccount(userData, out IsDuplicate);
                    ibase.Status = ResponseStatus.Successful;
                    ibase.AuthToken = userData.AuthToken;
                    ibase.FarmId = (userData.FarmId) >default(int) ? string.Empty : Convert.ToString(EncryptionHelper.AesEncryption(userData.FarmId.ToString(), EncryptionKey.LOG));
                    if (res==default(bool))
                    {
                        ibase.Status = ResponseStatus.Failed;
                        ibase.Error = (IsDuplicate == true ? "Sorry! A User exists with same Email" : "Registration failed");
                    }
                    repo.Dispose();
                    AggieGlobalLogManager.Info("RegistrationController :: Registration Completed ");
                }
            }
            catch(Exception ex)
            {
                ibase.Error = "Registration failed || " + ex.Message;
                AggieGlobalLogManager.Fatal("RegistrationController :: Register failed :: " + ex.Message);
            }

            return ibase;
        }

        [HttpGet]
        [ActionName("SignIn")]
        public AccountResponse SignIn(string username, string password, string userDeviceId)
        {
            bool res = default(bool);
            AccountResponse ibase = new AccountResponse();
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ibase.Status = ResponseStatus.Failed;
                    ibase.Error = "Required parameters not set";
                }
                else
                {
                    AggieGlobalLogManager.Info("RegistrationController :: SignIn started ");
                    var connectionString = "AggieGlobal";
                    var repo = new AccountManager(connectionString);
                    ibase.Status = ResponseStatus.Successful;
                    ibase.AuthToken = repo.LoginCheck(username, password, userDeviceId);
                    if(string.IsNullOrEmpty(ibase.AuthToken))
                    {
                        ibase.Error = "Invalid credentials";
                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                ibase.Status = ResponseStatus.Failed;
                ibase.Error = "Login failed || " + ex.Message;
                AggieGlobalLogManager.Fatal("RegistrationController :: SignIn failed :: " + ex.Message);
            }
            return ibase;
        }

        [ActionName("Delete")]
        public bool Delete()
        {
            bool ret = default(bool);

            return ret;
        }



    }

}
