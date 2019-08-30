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

using System;
using System.Collections.Generic;
using System.Data.Common;
using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Models.Client;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface IActivityRepository
    {
        bool CreateActivity(ActivityDetail prodData);
        bool UpdateActivity(ActivityDetail prodData);
        bool DeleteActivity(int productId);
        IEnumerable<ActivityDetail> GetActivityListByMonth(int userID, string dateStamp);
        IEnumerable<ActivityDetail> GetActivityCountByDate(int userID, string dateStamp);
        IEnumerable<ActivityDetail> GetActivityById(int ActivityId);
    }
}