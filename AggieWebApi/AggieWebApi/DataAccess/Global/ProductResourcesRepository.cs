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
    internal class ProductRessourcesRepository : SqlDataAccessRepository<ProductResources>, IProductRessourcesRepository
    {
        public ProductRessourcesRepository()
        {

        }

        public ProductRessourcesRepository(DbTransaction trans)
           : base(trans)
        {

        }
        public IEnumerable<ProductResources> GetProductResourcesList()
        {
            IEnumerable<ProductResources> listData = null;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    listData = GetRecord("GetProductResourcesList", AuthenticationSuccessmode);
                }
            }
            catch(Exception ex)
            {
                AggieGlobalLogManager.Fatal("ProductRepository :: GetActivityListByMonth failed :: " + ex.Message);
            }
            return listData;





        }


    }
}