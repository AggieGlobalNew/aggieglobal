﻿/* ========================================================================
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business
{
    public interface IPlotManager
    {
        bool CreateUpdatePlot(PlotDetail requestData);
        IEnumerable<PlotDetail> PlotListDetails(string farmid, int UserId);
        IList<SoilDataResponse> GetSoilDetails();
        IEnumerable<PlotDetail> GetPlotDetailsById(string plotid);

    }
}