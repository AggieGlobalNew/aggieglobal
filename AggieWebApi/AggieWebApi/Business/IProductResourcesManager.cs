using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business.Manager
{
    public interface IProductResourcesManager : IBusinessManager
    {
        IEnumerable<ProductResources> GetProductResourcesList();
        IEnumerable<ActivityDescriptions> GetAllActivityDescriptions();

    }
}