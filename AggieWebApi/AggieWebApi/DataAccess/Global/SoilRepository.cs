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
using System.Globalization;
using System.Linq;
using System.Web;

namespace AggieWebApi.DataAccess.Global
{
    internal class SoilRepository : SqlDataAccessRepository<SoilDetail>, ISoilRepository
    {

        public SoilRepository()
        {
        }

        public SoilRepository(DbTransaction trans)
            : base(trans)
        {
        }

        public IEnumerable<SoilDetailResponse> GetSoilDetails()
        {
            bool res = default(bool);
            IEnumerable<SoilDetail> SoilDetail = null;
            IList<SoilDetailResponse> SoilDetailResponse = new List<SoilDetailResponse>(); ;
            int OpMode = default(int);
            int _farmId = default(int);
            string LoginTokenKey = string.Empty;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    SoilDetail = GetRecord("GetSoilListDetails");
                    foreach(SoilDetail det in SoilDetail)
                    {
                        SoilDetailResponse response = new SoilDetailResponse();
                        response.SoilId = EncryptionHelper.AesEncryption(det.SoilId.ToString(), EncryptionKey.LOG);
                        response.SoilName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(det.SoilName.Trim());
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