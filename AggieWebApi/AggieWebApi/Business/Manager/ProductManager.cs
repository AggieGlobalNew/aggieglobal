using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Business;
using AggieGlobal.WebApi.Business.Managers;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business.Manager
{
    internal class ProductManager : ManagerBase, IProductManager
    {
        #region Member Variables


        private readonly IGlobalApp _globalApp = null;

        protected Account CurrentUser { get; set; }
        #endregion

        #region Constructor
        public ProductManager(string dbConnectionStringName)
            : base(dbConnectionStringName)
        {
            this._globalApp = GlobalApp.Instance;
            //this.CurrentUser = userIdentity;
        }
        #endregion


        public IEnumerable<CategoryMaster> GetCategoryList()
        {

            bool result = default(bool);
            try
            {
                return new RepositoryCreator().CatgeoryRepository.GetCategoryList();
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ProductManager :: GetCategoryList failed :: " + ex.Message);
            }
            return null;
        }
        public int CreateSubCategory(ProductDetail detail)
        {
            try
            {
                return new RepositoryCreator().ProductRepository.CreateSubCategory(detail);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ProductManager :: CreateSubCategory failed :: " + ex.Message);
            }
            return default(int);
        }


        public IEnumerable<ProductDetail> GetSubCategoryList(int ProductTypeId,int userid)
        {

            bool result = default(bool);
            try
            {
                return new RepositoryCreator().ProductRepository.GetSubCategoryList(ProductTypeId, userid);
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ProductManager :: GetSubCategoryList failed :: " + ex.Message);
            }
            return null;
        }





    }

}