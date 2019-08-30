using AggieGlobal.WebApi;
using AggieGlobal.WebApi.Business;
using AggieGlobal.WebApi.Common;
using AggieWebApi.Business.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using WebAPI.Sample;

namespace AggieWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private IGlobalApp _theGlobalApp = null;
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);


            //Http Client Protocol Settings 
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.MaxServicePointIdleTime = 60 * 60 * 1000;//60 Minutes
            ServicePointManager.DefaultConnectionLimit = 132;


            FileInfo fi = new FileInfo(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\web.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fi);

            Assembly assem = Assembly.GetExecutingAssembly();
            AssemblyName assemName = assem.GetName();

            AggieGlobalLogManager.Info("=================================================================");
            AggieGlobalLogManager.Info("Starting SKYSITE-Web-API-Service v{0}", assemName.Version);
            AggieGlobalLogManager.Info("=================================================================");

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            FilterConfig.RegisterHttpFilters(GlobalConfiguration.Configuration.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);


            FormatterConfig.RegisterFormatters(GlobalConfiguration.Configuration.Formatters);
            HandlerConfig.RegisterHandlers(GlobalConfiguration.Configuration.MessageHandlers);

            //Instnatiate the one & only instance of GlobalApp - facade design pattern to Provide an unified interface to a set of interfaces to deal with business functions
            if (_theGlobalApp == null)
            {
                string dbConnectionStringName = "AggieGlobalDB";
                GlobalApp.Initialize(dbConnectionStringName);
                _theGlobalApp = GlobalApp.Instance;
            }
        }

        protected void Application_End()
        {
            AggieGlobalLogManager.Info("=================================================================");
            AggieGlobalLogManager.Info("Exiting SKYSITE-Web-API-Service.");
            AggieGlobalLogManager.Info("=================================================================");
        }

        public override void Init()
        {
            this.PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(
                SessionStateBehavior.Required);
        }
    }
}
