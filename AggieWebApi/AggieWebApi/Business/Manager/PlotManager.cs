using AggieGlobal.Models.Client;
using AggieGlobal.Models.Common;
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
    internal class PlotManager : ManagerBase, IPlotManager
    {
        private IGlobalApp _globalApp;

        #region Constructor
        public PlotManager()
        {

        }
        public PlotManager(string dbConnectionStringName)
            : base(dbConnectionStringName)
        {
            this._globalApp = GlobalApp.Instance;
            //this.CurrentUser = userIdentity;
        }
        #endregion


        public bool CreateUpdatePlot(PlotDetail requestData)
        {

            bool result = default(bool);
            bool res = default(bool);
            try
            {
                return new RepositoryCreator().PlotRepository.CreateUpdatePlot(requestData);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("PlotManager :: CreateUpdatePlot failed :: " + ex.Message);
            }
            return result;
        }

        public IEnumerable<PlotDetail> GetPlotDetailsById(string plotid)
        {
            IEnumerable<FarmDetail> result = null;
            try
            {
                return new RepositoryCreator().PlotRepository.GetPlotDetailsById(plotid);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("PlotManager :: GetPlotDetails failed :: " + ex.Message);
            }
            return null;
        }


        public IEnumerable<PlotDetail> PlotListDetails(string farmid, int UserId)
        {
            IEnumerable<FarmDetail> result = null;
            try
            {
                return new RepositoryCreator().PlotRepository.PlotListDetails(farmid, UserId);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("PlotManager :: GetPlotDetails failed :: " + ex.Message);
            }
            return null;
        }

        public IList<SoilDataResponse> GetSoilDetails()
        {
            IList<SoilDataResponse> dataresponse = new List<SoilDataResponse>();
            try
            {
                SoilDataResponse responseAll = new SoilDataResponse();
                responseAll.soildetail =  new RepositoryCreator().SoilRepository.GetSoilDetails();
                responseAll.soilphdetail = new RepositoryCreator().SoilPhRepository.GetSoilPhDetails();
                responseAll.Status = ResponseStatus.Successful;
                dataresponse.Add(responseAll);
            }
            catch (Exception e) { AggieGlobalLogManager.Fatal("AccountRepository :: LoginCheck failed :: " + e.Message); }

            return dataresponse;
        }


    }
}