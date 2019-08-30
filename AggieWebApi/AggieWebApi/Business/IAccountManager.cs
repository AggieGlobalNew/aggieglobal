using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business.Manager
{
    public interface IAccountManager : IBusinessManager
    {
        bool CreateAccount(Account userdata,out bool IsDuplicate);
        string LoginCheck(string username, string password, string userDeviceId);
        Account GetAcountByUserId(string Email, int userId, string DeviceId);
        int UpdateSession(string Email, int userId, string DeviceId);
    }
}