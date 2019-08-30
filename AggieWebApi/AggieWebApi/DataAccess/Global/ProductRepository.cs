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
    internal class ProductRepository : SqlDataAccessRepository<ProductDetail>, IProductRepository
    {

        public ProductRepository()
        {

        }

        public ProductRepository(DbTransaction trans)
            : base(trans)
        {

        }

        public int CreateSubCategory(ProductDetail detail)
        {
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    detail.ProductId = CreateRecord("CreateSubCategory", detail.ProductName, detail.ProductImageLocation, detail.CategoryID,detail.UserId,AuthenticationSuccessmode);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ProductRepository :: GetProductListByUser failed :: " + ex.Message);
            }
            return detail.ProductId;
        }

        public IEnumerable<ProductDetail> GetProductListByUser(int userID, DateTime dateStamp)
        {
            IEnumerable<ProductDetail> proddetail = null;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    if (dateStamp == null)
                        proddetail = GetRecord("GetProductListByUser", userID, AuthenticationSuccessmode);
                    else
                        proddetail = GetRecord("GetProductListByDate", userID, dateStamp, AuthenticationSuccessmode);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ProductRepository :: GetProductListByUser failed :: " + ex.Message);
            }
            return proddetail;
        }
        public IEnumerable<ProductDetail> GetSubCategoryList(int ProductTypeId,int userId)
        {

            IEnumerable<ProductDetail> proddetail = null;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    proddetail = GetRecord("GetSubCategoryList", ProductTypeId, userId, AuthenticationSuccessmode);

                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ProductRepository :: GetSubCategoryList failed :: " + ex.Message);
            }
            return proddetail;


        }
    }
}