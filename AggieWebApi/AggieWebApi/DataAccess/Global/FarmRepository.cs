using AggieGlobal.Models.Client;
using AggieGlobal.Models.Common;
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
    internal class FarmRepository : SqlDataAccessRepository<FarmDetail>, IFarmRepository
    {

        public int CreateUpdateFarm(FarmDetail requestData)
        {
            string LoginTokenKey = string.Empty;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    requestData.CoOpId = (requestData.CoOpId == 0 ? 3 : requestData.CoOpId);
                    requestData.IsActive = true;
                    if (requestData.FarmId == default(int))
                    {
                        AuthenticationSuccessmode = CreateRecord("CreateFarmDetail", requestData.FarmName, requestData.CoOpId, requestData.FarmSize, requestData.FarmSizeUnit, requestData.FarmEstablishedDate, requestData.FarmAddress, requestData.IsActive);
                        requestData.FarmId = AuthenticationSuccessmode;
                    }
                    else
                        AuthenticationSuccessmode = UpdateRecord("UpdateFarmDetail", requestData.FarmName, requestData.CoOpId, requestData.FarmSize, requestData.FarmSizeUnit, requestData.FarmEstablishedDate, requestData.FarmAddress, requestData.FarmId, requestData.IsActive);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: CreateUpdateFarm :: " + ex.Message);
            }
            return AuthenticationSuccessmode;
        }
        public bool MapFarmByUserDetail(int FarmId, int UserId)
        {
            bool result = default(bool);
            int AuthenticationSuccessmode = 0;
            try
            {
                //DbTransaction transaction = null;
                using (var connection = GetConnection())
                {
                    connection.Open();
                    AuthenticationSuccessmode = UpdateRecord("MapFarmByUserDetail", FarmId, UserId, AuthenticationSuccessmode);
                    result = (AuthenticationSuccessmode == 1 ? true : false);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: CreateUpdateFarm :: " + ex.Message);
            }
            return result;
        }
        public IEnumerable<FarmDetail> GetFarmDetails(int userid)
        {
            bool res = default(bool);
            IEnumerable<FarmDetail> farmDetail = null;
            int OpMode = default(int);
            string LoginTokenKey = string.Empty;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    farmDetail = GetRecord("GetFarmDetails", userid, AuthenticationSuccessmode);

                }
                if (farmDetail != null && farmDetail.Count() > default(int))
                {

                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: LoginCheck failed :: " + ex.Message);
            }
            return farmDetail;
        }
        public IEnumerable<FarmDetail> GetInActiveFarmDetails()
        {
            bool res = default(bool);
            IEnumerable<FarmDetail> farmDetail = null;
            int OpMode = default(int);
            string LoginTokenKey = string.Empty;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    farmDetail = GetRecord("GetInActiveFarmDetails", AuthenticationSuccessmode);

                }
                if (farmDetail != null && farmDetail.Count() > default(int))
                {

                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("AccountRepository :: LoginCheck failed :: " + ex.Message);
            }
            return farmDetail;
        }
        public IList<SoilDataResponse> GetSoilDetails()
        {
            IList<SoilDataResponse> dataresponse = new List<SoilDataResponse>();
            try
            {


            } catch (Exception e) { AggieGlobalLogManager.Fatal("AccountRepository :: LoginCheck failed :: " + e.Message); }

            return dataresponse;
        }
    }
}