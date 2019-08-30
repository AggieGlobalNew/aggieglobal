using AggieGlobal.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface IPlotRepository
    {
        IEnumerable<PlotDetail> PlotListDetails(string farmid,int UserId);
        bool CreateUpdatePlot(PlotDetail requestData);
        IEnumerable<PlotDetail> GetPlotDetailsById(string plotid);
    }
}