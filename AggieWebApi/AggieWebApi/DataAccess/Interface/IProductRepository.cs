using System;
using System.Collections.Generic;
using System.Data.Common;
using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Models.Client;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface IProductRepository
    {
        int CreateSubCategory(ProductDetail detail);
        IEnumerable<ProductDetail> GetProductListByUser(int userID, DateTime dateStamp);
        IEnumerable<ProductDetail> GetSubCategoryList(int ProductTypeId, int userId);

    }
}