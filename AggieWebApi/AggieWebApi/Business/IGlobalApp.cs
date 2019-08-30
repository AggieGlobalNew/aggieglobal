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


using AggieGlobal.Business.Manager;
using AggieGlobal.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business.Manager
{
    public interface IGlobalApp : IDisposable
    {
        IAccountManager GetAccountManager(Account currentUser);
        IActivityManager GetActivityManager(Account currentUser);
        IFarmManager GetFarmManager(Account currentUser);
        IPlotManager GetPlotManager(Account currentUser);
        IProductManager GetProductManager(Account currentUser);
        IProductResourcesManager GetProductResourcesManager(Account currentUser);
    }
}