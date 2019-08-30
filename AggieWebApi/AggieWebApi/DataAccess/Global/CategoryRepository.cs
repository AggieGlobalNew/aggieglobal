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
    internal class CategoryRepository : SqlDataAccessRepository<CategoryMaster>, ICategoryRepository
    {

        public IEnumerable<CategoryMaster> GetCategoryList()
        {
            IEnumerable<CategoryMaster> categorydetail = null;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    categorydetail = GetRecord("GetCategoryList", AuthenticationSuccessmode);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("PlotRepository :: GetPlotDetailsById failed :: " + ex.Message);
            }
            return categorydetail;
        }
    }
}