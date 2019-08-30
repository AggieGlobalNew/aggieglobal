using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Business;
using AggieGlobal.WebApi.Business.Managers;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business.Manager
{
    internal class ActivityManager : ManagerBase, IActivityManager
    {
        #region Member Variables


        private readonly IGlobalApp _globalApp = null;

        protected Account CurrentUser { get; set; }
        #endregion

        #region Constructor
        public ActivityManager(string dbConnectionStringName)
            : base(dbConnectionStringName)
        {
            this._globalApp = GlobalApp.Instance;
            //this.CurrentUser = userIdentity;
        }
        #endregion


        public bool CreateUpdateActivity(ActivityDetail requestData)
        {

            bool result = default(bool);
            bool res = default(bool);
            try
            {
                if (requestData.ActivityId == default(int))
                    return new RepositoryCreator().ActivityRepository.CreateActivity(requestData);
                else
                    return new RepositoryCreator().ActivityRepository.UpdateActivity(requestData);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ActivityManager :: CreateUpdateActivity failed :: " + ex.Message);
            }
            return result;
        }
        public bool DeleteActivity(int ProductId)
        {

            bool result = default(bool);
            try
            {
                if (ProductId > default(int))
                    return new RepositoryCreator().ActivityRepository.DeleteActivity(ProductId);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ActivityManager :: CreateUpdateActivity failed :: " + ex.Message);
            }
            return result;
        }
        public IEnumerable<ActivityDetail> GetActivityListByMonth(int userID, string dateStamp)
        {
            try
            {
                if (userID > default(int) && !string.IsNullOrEmpty(dateStamp))
                    return new RepositoryCreator().ActivityRepository.GetActivityListByMonth(userID,dateStamp);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ActivityManager :: GetActivityListByMonth failed :: " + ex.Message);
            }
            return null;
        }
        public IEnumerable<ActivityDetail> GetActivityCountByDate(int userID,string dateStamp)
        {
            try
            {
                if (userID > default(int))
                    return new RepositoryCreator().ActivityRepository.GetActivityCountByDate(userID, dateStamp);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ActivityManager :: GetActivityCountByDate failed :: " + ex.Message);
            }
            return null;
        }
        public IEnumerable<ActivityDetail> GetActivityById(int ActivityId)
        {
            try
            {
                if (ActivityId > default(int))
                    return new RepositoryCreator().ActivityRepository.GetActivityById(ActivityId);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ActivityManager :: GetActivityById failed :: " + ex.Message);
            }
            return null;
        }
    }
}