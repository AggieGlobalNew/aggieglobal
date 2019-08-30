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
