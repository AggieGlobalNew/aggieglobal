using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Xml.Linq;

namespace AggieGlobal.WebApi.Common
{
    public static class GeneralUtility
    {
        public static string CurrentDirectory
        {
            get
            {
                return HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/") : (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AppLogicalPath"].ToString()) ? ConfigurationManager.AppSettings["AppLogicalPath"].ToString() : Environment.CurrentDirectory);
            }
        }

        /// <summary>
        /// This static method has been created to convert String date time to proper DateTime format
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(string dateTime)
        {
            char[] delimiterChars = { ' ', '/', ':','-' };
            string[] words = dateTime.Split(delimiterChars);
            if (words.Length > 6 && words[6].ToUpperInvariant() == "AM" && words[3] == "12")
                words[3] = "00";

            DateTime startDate = new DateTime();
            if (DateTime.TryParseExact(dateTime, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startDate))
            {
                
            }
            else
            {
                //startDate = DateTime.ParseExact(item.SelectedValue.Split('|')[0], "MM/dd/yyyy", null);
                startDate = new DateTime(int.Parse(words[2]), int.Parse(words[0]), int.Parse(words[1]), int.Parse(words[3]), int.Parse(words[4]), int.Parse(words[5]));
            }


            return startDate;
        }

        /// <summary>
        /// This static method has been created to convert any generic object to list
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static List<T> AnyObjectToList<T>(this object Input)
        {
            if (Input is System.Collections.IEnumerable)
                return ((System.Collections.IEnumerable)Input).Cast<T>().ToList();
            return new List<T>() { (T)Input };
        }
    }
}
