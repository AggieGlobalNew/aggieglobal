using AggieGlobal.Business.Manager;
using AggieGlobal.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business.Manager
{
    public interface IGlobalApp : IDisposable
    {
        IAccountManager GetAccountManager(Account currentUser);
        IActivityManager GetActivityManager(Account currentUser);
        IFarmManager GetFarmManager(Account currentUser);
        IPlotManager GetPlotManager(Account currentUser);
        IProductManager GetProductManager(Account currentUser);
        IProductResourcesManager GetProductResourcesManager(Account currentUser);
    }
}