using System;
using System.Web;
using System.Web.Http;
using AggieGlobal.Models.Client;
using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.Controllers.Common;
using AggieGlobal.WebApi.DataAccess.Common;
using System.IO;

using AggieGlobal.WebApi.Filters;
using AggieGlobal.WebApi.Infrastructure;
using AggieWebApi.Business.Manager;

namespace AggieWebApi.Controllers
{
    [CustomFilter]
    public class CommonController :  AbstractController<CommonModuleResponse>
    {
        public CommonController()
           : this(null)
        {

        }

        public CommonController(IAccountRepository aRepository)
            : base()
        { }

        [HttpPost]
        public CommonModuleResponse DownloadFile(string relativePath)
        {
            Stream file = null;
            var myFile = System.Web.Hosting.HostingEnvironment.MapPath("~/ModuleImg/" + relativePath);
            FileStream fs = null;
            bool NoImageRequired = relativePath == "nash.png" ? false : true;

            CommonModuleResponse response = new CommonModuleResponse();
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    fs = new FileStream(myFile, FileMode.Open, FileAccess.Read, FileShare.None);
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                    try
                    {
                        byte[] buffer = new byte[16 * 1024];
                        byte[] storeArr = null;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            response.fileStream = ms.ToArray();
                            response.Status = ResponseStatus.Successful;
                            if (response.fileStream.Length == 0)
                            { if (NoImageRequired == true) DownloadFile("nash.png"); }
                            else if (response.fileStream.Length == 0 && NoImageRequired == false)
                            {
                                response.Status = ResponseStatus.Failed;
                                response.Error = "Unable to download file";
                            }
                        }
                        fs.Close();
                        fs.Dispose();

                    }
                    catch (Exception ex)
                    {
                        if (fs != null)
                        {
                            fs.Close();
                            fs.Dispose();
                        }
                        if (NoImageRequired == true) DownloadFile("nash.png");
                        response.Status = ResponseStatus.Failed;
                        response.Error = "Unable to download file";
                        AggieGlobalLogManager.Fatal("CommonController :: DownloadFile failed :: " + ex.Message);

                    }
                }
            }
            catch (Exception e)
            {
                if (NoImageRequired == true) { DownloadFile("nash.png"); }
                else
                {
                    response.Status = ResponseStatus.Failed;
                    response.Error = "Unable to download file";
                }
                AggieGlobalLogManager.Fatal("CommonController :: DownloadFile failed :: " + e.Message);
            }
            return response;
        }


    }
}