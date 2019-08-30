using AggieGlobal.Business.Manager;
using AggieGlobal.Models.Client;
using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Business;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.Controllers.Common;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Filters;
using AggieGlobal.WebApi.Infrastructure;
using AggieWebApi.Business.Manager;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace AggieWebApi.Controllers
{
    [CustomFilter]
    public class UserAccountController : AbstractController<Account>
    {
        public UserAccountController()
           : base()
        { }
        public UserAccountController(IAccountRepository aRepository)
            : base()
        { }

        [HttpPost]
        [ActionName("GetUserDetails")]
        public AccountResponse GetUserDetails()
        {
            AccountResponse userData = null;
            var httpContent = Request.Content;
            try
            {
                string userId = string.Empty;

                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                    AggieGlobalLogManager.Info("FarmDetailsController :: GetFarmsDetails started ");
                    var connectionString = "AggieGlobal";
                    var repo = new AccountManager(connectionString);
                    userData = repo.GetAcountByUserClientData(sessionObject._email, sessionObject._userId, sessionObject._deviceid);
                    userData.Status = ResponseStatus.Successful;
                    return userData;
                }
                else
                {
                    userData = new AccountResponse();
                    userData.Error = "Invalid credentials";
                    userData.Status = ResponseStatus.Failed;
                }
            }
            catch (Exception ex)
            {
                userData = new AccountResponse();
                userData.Error = "failed to retreive user data";
                userData.Status = ResponseStatus.Failed;
                AggieGlobalLogManager.Fatal("FarmDetailsController :: GetFarmsDetails failed :: " + ex.Message);
            }
            return userData;
        }

        [ActionName("UpdateSession")]
        public AccountResponse UpdateSession()
        {
            int ret = default(int);
            AccountResponse ibase = new AccountResponse();
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                    AggieGlobalLogManager.Info("FarmDetailsController :: GetFarmsDetails started ");
                    var connectionString = "AggieGlobal";
                    var repo = new AccountManager(connectionString);
                    ret = repo.UpdateSession(sessionObject._email, sessionObject._userId, sessionObject._deviceid);
                    if (ret == 0)
                    {
                        ibase.Status = ResponseStatus.Successful;
                        ibase.Error = "Invalid credentials";
                    }

                }
            }
            catch (Exception ex)
            {
                ibase.Status = ResponseStatus.Failed;
                ibase.Error = "Invalid credentials";
                AggieGlobalLogManager.Fatal("FarmDetailsController :: GetFarmsDetails failed :: " + ex.Message);
            }
            return ibase;
        }
    }
}
