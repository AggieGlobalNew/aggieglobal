using AggieGlobal.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business
{
    public interface  IFarmManager :IDisposable
    {
        int CreateUpdateFarm(FarmDetail requestData);
        int CreateUpdateFarmAndPlot(FarmDetail requestData);
        IEnumerable<FarmDetail> GetFarmDetails(int userId);
        bool MapFarmByUserDetail(int FarmId, int UserId);
        IEnumerable<FarmDetail> GetInActiveFarmDetails();
    }
}