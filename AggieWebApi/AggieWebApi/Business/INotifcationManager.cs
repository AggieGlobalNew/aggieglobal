using AggieGlobal.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieWebApi.Business
{
    public interface INotifcationManager
    {
        void Email(string htmlString);
    }
}