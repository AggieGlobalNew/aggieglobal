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
    internal class FarmManager : ManagerBase, IFarmManager
    {
        private IGlobalApp _globalApp;

        #region Constructor
        public FarmManager(string dbConnectionStringName)
            : base(dbConnectionStringName)
        {
            this._globalApp = GlobalApp.Instance;
            //this.CurrentUser = userIdentity;
        }
        #endregion

        public int CreateUpdateFarm(FarmDetail requestData)
        {

            try
            {
                return new RepositoryCreator().FarmRepository.CreateUpdateFarm(requestData);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("FarmManager :: CreateUpdateFarm failed :: " + ex.Message);
            }
            return requestData.FarmId;
        }

        public int CreateUpdateFarmAndPlot(FarmDetail requestData)
        {

            try
            {
                return new RepositoryCreator().FarmRepository.CreateUpdateFarm(requestData);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("FarmManager :: CreateUpdateFarmAndPlot failed :: " + ex.Message);
            }
            return requestData.FarmId;
        }




        public IEnumerable<FarmDetail> GetFarmDetails(int userId)
        {
            IEnumerable<FarmDetail> result = null;
            bool res = default(bool);
            try
            {
                return new RepositoryCreator().FarmRepository.GetFarmDetails(userId);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("FarmManager :: GetFarmDetails failed :: " + ex.Message);
            }
            return null;
        }

        public IEnumerable<FarmDetail> GetInActiveFarmDetails()
        {
            IEnumerable<FarmDetail> result = null;
            bool res = default(bool);
            try
            {
                return new RepositoryCreator().FarmRepository.GetInActiveFarmDetails();
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("FarmManager :: GetFarmDetails failed :: " + ex.Message);
            }
            return null;
        }

        public bool MapFarmByUserDetail(int FarmId, int UserId)
        {
            bool res = default(bool);
            try
            {
                return new RepositoryCreator().FarmRepository.MapFarmByUserDetail(FarmId,UserId);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("FarmManager :: GetFarmDetails failed :: " + ex.Message);
            }
            return res;
        }


        public void Dispose()
        {

        }
    }
}