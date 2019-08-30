using AggieGlobal.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business
{
    public interface IActivityManager : IDisposable
    {
        bool CreateUpdateActivity(ActivityDetail requestData);
        IEnumerable<ActivityDetail> GetActivityListByMonth(int userID, string dateStamp);
        IEnumerable<ActivityDetail> GetActivityCountByDate(int userID, string dateStamp);
        bool DeleteActivity(int ProductId);
        IEnumerable<ActivityDetail> GetActivityById(int ActivityId);
    }
}