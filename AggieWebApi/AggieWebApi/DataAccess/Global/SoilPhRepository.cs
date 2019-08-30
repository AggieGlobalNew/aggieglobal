/* ========================================================================
 * Includes    : FormatterConfig.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Implements various data type formatter registration logics for AggieGlobal.WebApi.
 * Copyright © 2019 Atlas Software Technologies All rights reserved
 * ========================================================================
 * Release History
 * ---------------
 * DESCRIPTION					CREATED / MODIFIED BY
 * -----------                  ---------------------
 * Initial Implementation		Sudipta Sarkar
 * ========================================================================
 */

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
    internal class SoilPhRepository : SqlDataAccessRepository<SoilPhDetail>, ISoilPhRepository
    {


        public SoilPhRepository()
        {

        }

        public SoilPhRepository(DbTransaction trans)
            : base(trans)
        {
        }

        public IEnumerable<SoilPhDetailResponse> GetSoilPhDetails()
        {
            bool res = default(bool);
            IEnumerable<SoilPhDetail> SoilDetail = null;
            IList<SoilPhDetailResponse> SoilDetailResponse = new List<SoilPhDetailResponse>(); ;
            int OpMode = default(int);
            int _farmId = default(int);
            string LoginTokenKey = string.Empty;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    SoilDetail = GetRecord("GetSoilPhListDetails");
                    foreach (SoilPhDetail det in SoilDetail)
                    {
                        SoilPhDetailResponse response = new SoilPhDetailResponse();
                        response.SoilPhId = EncryptionHelper.AesEncryption(det.SoilPhId.ToString(), EncryptionKey.LOG);
                        response.SoilPhvalue = det.SoilPhvalue;
                        response.Status = ResponseStatus.Successful;
                        SoilDetailResponse.Add(response);

                    }
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("PlotRepository :: SoilRepository failed :: " + ex.Message);
            }
            return SoilDetailResponse;
        }
    }
}