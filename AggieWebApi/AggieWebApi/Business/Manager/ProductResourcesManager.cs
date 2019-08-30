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
using AggieGlobal.WebApi.Business;
using AggieGlobal.WebApi.Business.Managers;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieWebApi.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.Business.Manager
{
    internal class ProductResourcesManager : ManagerBase, IProductResourcesManager
    {
        #region Member Variables


        private readonly IGlobalApp _globalApp = null;

        protected Account CurrentUser { get; set; }
        #endregion



        #region Constructor
        public ProductResourcesManager(string dbConnectionStringName)
            : base(dbConnectionStringName)
        {
            this._globalApp = GlobalApp.Instance;
            //this.CurrentUser = userIdentity;
        }
        #endregion


        public IEnumerable<ProductResources> GetProductResourcesList()
        {
            try
            {
                return new RepositoryCreator().ProductRessourcesRepository.GetProductResourcesList();
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ProductResourcesManager :: GetProductResourcesList failed :: " + ex.Message);
            }
            return null;
        }
        public IEnumerable<ActivityDescriptions> GetAllActivityDescriptions()
        {
            try
            {
                return new RepositoryCreator().ActivityDescriptionRepository.GetAllActivityDescriptions();
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ProductResourcesManager :: GetAllActivityDescriptions failed :: " + ex.Message);
            }
            return null;
        }

    }
}