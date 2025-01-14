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

using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.Models.Client
{
    public class FarmDetail : ModelBase
    {
        public int FarmId { get; set; }

        public string FarmName { get; set; }

        public int CoOpId { get; set; }

        public int FarmSize { get; set; }

        public string FarmSizeUnit { get; set; }

        public DateTime FarmEstablishedDate { get; set; }

        public string FarmAddress { get; set; }

        public string CoOpName { get; set; }

        public bool IsActive { get; set; }

    }
    public class CoOperativeDetail : ModelBase
    {
        public int CoOpId { get; set; }
        public string CoOpName { get; set; }
    }
    public class FarmPlotDetail : ModelBase
    {
        public int FarmId { get; set; }

        public string FarmName { get; set; }

        public int CoOpId { get; set; }

        public int FarmSize { get; set; }

        public string FarmSizeUnit { get; set; }

        public DateTime FarmEstablishedDate { get; set; }

        public string FarmAddress { get; set; }

        public string CoOpName { get; set; }
    }


    public class FarmDetailResponse : ModelBase
    {

        public string FarmName { get; set; }

        public int FarmSize { get; set; }

        public string FarmSizeUnit { get; set; }

        public DateTime FarmEstablishedDate { get; set; }

        public string FarmAddress { get; set; }

        public string CoOpName { get; set; }

        public string FarmId { get; set; }
    }


}
