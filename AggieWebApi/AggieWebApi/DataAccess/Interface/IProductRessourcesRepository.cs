using System;
using System.Collections.Generic;
using System.Data.Common;
using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Models.Client;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface IProductRessourcesRepository
    {
        IEnumerable<ProductResources> GetProductResourcesList();
    }
}