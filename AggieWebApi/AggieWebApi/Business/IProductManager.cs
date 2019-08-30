using AggieGlobal.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business
{
    public interface IProductManager
    {
        IEnumerable<CategoryMaster> GetCategoryList();
        IEnumerable<ProductDetail> GetSubCategoryList(int ProductTypeId, int userid);
    }
}