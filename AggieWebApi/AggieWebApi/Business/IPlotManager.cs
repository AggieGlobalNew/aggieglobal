using AggieGlobal.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business
{
    public interface IPlotManager
    {
        bool CreateUpdatePlot(PlotDetail requestData);
        IEnumerable<PlotDetail> PlotListDetails(string farmid, int UserId);
        IList<SoilDataResponse> GetSoilDetails();
        IEnumerable<PlotDetail> GetPlotDetailsById(string plotid);

    }
}