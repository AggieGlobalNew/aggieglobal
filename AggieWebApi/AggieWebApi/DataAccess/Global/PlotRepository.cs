using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace AggieWebApi.DataAccess.Global
{
    internal class PlotRepository : SqlDataAccessRepository<PlotDetail>, IPlotRepository
    {

        public IEnumerable<PlotDetail> PlotListDetails(string farmid,int UserId)
        {
            bool res = default(bool);
            IEnumerable<PlotDetail> farmDetail = null;
            int _farmId = default(int);
            string LoginTokenKey = string.Empty;
            int AuthenticationSuccessmode = 0;
            try
            {
                _farmId = Convert.ToInt32(EncryptionHelper.AesDecryption(farmid, EncryptionKey.LOG));
                using (var connection = GetConnection())
                {
                    connection.Open();
                    farmDetail = GetRecord("GetPlotListDetails", _farmId, UserId, AuthenticationSuccessmode);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("PlotRepository :: GetFarmDetails failed :: " + ex.Message);
            }
            return farmDetail;
        }
        public IEnumerable<PlotDetail> GetPlotDetailsById(string plotid)
        {
            IEnumerable<PlotDetail> plotdetail = null;
            int AuthenticationSuccessmode = 0;
            int _plotId = Convert.ToInt32(EncryptionHelper.AesDecryption(plotid, EncryptionKey.LOG));
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    plotdetail = GetRecord("GetPlotDetailsById", _plotId, AuthenticationSuccessmode);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("PlotRepository :: GetPlotDetailsById failed :: " + ex.Message);
            }
            return plotdetail;
        }
        public bool CreateUpdatePlot(PlotDetail requestData)
        {
            string LoginTokenKey = string.Empty;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    if (requestData.PlotId == 0)
                    {
                        AuthenticationSuccessmode=CreateRecord("CreatePlot", requestData.FarmId, requestData.PlotName, requestData.PlotSize, requestData.Organic, requestData.SoilPhId, requestData.SoilId, requestData.Notes, requestData.UserID, AuthenticationSuccessmode);
                        requestData.PlotId = AuthenticationSuccessmode;
                    }
                    else
                        UpdateRecord("UpdatePlot", requestData.PlotName, requestData.PlotSize, requestData.Organic, requestData.SoilPhId, requestData.SoilId, requestData.Notes, requestData.FarmId, requestData.PlotId, AuthenticationSuccessmode);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: CreateUpdateFarm :: " + ex.Message);
            }
            return (requestData.PlotId == 1 ? true : false);
        }
    }
}