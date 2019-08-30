using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.Models.Client;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Configuration;
using System.Runtime.Caching;

namespace AggieGlobal.WebApi.Infrastructure
{
    public static class Utility
    {
        public static string CurrentDirectory
        {
            get
            {
                return HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/") : (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AppLogicalPath"].ToString()) ? ConfigurationManager.AppSettings["AppLogicalPath"].ToString() : Environment.CurrentDirectory);
            }
        }

        public static string GetCustomDescription(this Enum en)
        {
            FieldInfo fieldInfo = en.GetType().GetField(en.ToString());
            DescriptionAttribute[] descriptionAttribute =
                  (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                  typeof(DescriptionAttribute), false);
            return (descriptionAttribute.Length > 0) ? descriptionAttribute[0].Description : null;
        }

        public static long GetObjectSize(object o)
        {
            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, o);
                return s.Length;
            }
        }

        public static IDictionary<string, string> GetQueryParameters(string queryString)
        {
            var retval = new Dictionary<string, string>();
            foreach (var item in queryString.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var split = item.Split('=');
                retval.Add(split[0].ToLower(), split[1]);
            }

            return retval;
        }

        public static IDictionary<string, string> GetQueryParametersWithCase(string queryString)
        {
            var retval = new Dictionary<string, string>();
            foreach (var item in queryString.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var split = item.Split('=');
                retval.Add(split[0], split[1]);
            }

            return retval;
        }

        public static string SetQueryParameters(string queryString, IDictionary<string, string> queryParams)
        {
            string queryLoc = queryString.Split('?')[0];
            queryLoc += "?";
            foreach (var item in queryParams)
            {
                queryLoc += "&" + item.Key + "=" + item.Value;
            }

            return queryLoc;
        }

        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        public static string StripTagsRegexCompiled(string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        /// <summary>
        /// Performance test for HTML removal
        /// HtmlRemoval.StripTagsRegex:         2404 ms
        /// HtmlRemoval.StripTagsRegexCompiled: 1366 ms
        /// HtmlRemoval.StripTagsCharArray:      287 ms [fastest]
        /// 
        /// File length test for HTML removal
        /// File tested:                        Real-world HTML file
        /// File length before:                 8085 chars
        /// HtmlRemoval.StripTagsRegex:         4382 chars
        /// HtmlRemoval.StripTagsRegexCompiled: 4382 chars
        /// HtmlRemoval.StripTagsCharArray:     4382 chars
        public static string StripTagsCharArray(string source, int maxLength = -1)
        {
            int size = maxLength > 0 ? Math.Min(maxLength, source.Length) : source.Length;
            char[] array = new char[maxLength];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < size; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double ConvertSize(int source, double sourceValue, int target)
        {
            if (target > source) //divided by 1024 with power
            {
                double powerValue = Math.Pow(1024, target - source);
                sourceValue /= powerValue;
            }
            else if (target < source) //multiply by 1024 with power
            {
                double powerValue = Math.Pow(1024, source - target);
                sourceValue *= powerValue;
            }
            return sourceValue;
        }

        public static string GetDefaultUserIdAndPassword(int passwordLenth)
        {
            string allowedPasswordCharacter = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            Random randNum = new Random();
            char[] generatedPassword = new char[passwordLenth];
            int allowedCharCount = allowedPasswordCharacter.Length;

            for (int i = 0; i < passwordLenth; i++)
            {
                generatedPassword[i] = allowedPasswordCharacter[(int)((allowedPasswordCharacter.Length) * randNum.NextDouble())];
            }

            return new string(generatedPassword);
        }

        public static string AutomaticExtracted(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return "^1";
            else
                return tag + "^1";
        }

        public static string ManuallyExtracted(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return "^0";
            else
                return tag + "^0";
        }

        public static IDictionary<string, string> GetHeaderParameters(HttpRequestHeaders headers)
        {
            var retVal = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                retVal.Add(header.Key.ToLower(), ((string[])(header.Value))[0].ToString());
            }

            return retVal;
        }

        public static string ToCustomDateTimeMiliSecondString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static IList<string> GetOCRNotRequiredFileExtentions()
        {
            IList<string> lstOCRNotRequiredFileExt = null;
            string XmlPath = string.Empty;
            try
            {
                XmlPath = "OCRNotRequiredFileExt".GetConfigKeyValue();
                if (!string.IsNullOrEmpty(XmlPath) && File.Exists(XmlPath))
                {
                    XmlDocument objXmlDocument = new XmlDocument();
                    XmlNodeList childNodes = null;
                    objXmlDocument.Load(XmlPath);
                    childNodes = objXmlDocument.DocumentElement.ChildNodes;
                    lstOCRNotRequiredFileExt = new List<string>();

                    for (int i = 0; i < childNodes.Count; i++)
                    {
                        lstOCRNotRequiredFileExt.Add(childNodes[i].InnerText);
                    }
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Warn(ex, "Failed to get extensions for which OCR is not required!");
            }
            return lstOCRNotRequiredFileExt;
        }

        public static string GetConfigKeyValue(this string strInput)
        {
            if (!string.IsNullOrEmpty(strInput))
            {
                strInput = GetConfigData(strInput);
            }
            return strInput;
        }

        private static string GetConfigData(string Key)
        {
            string KeyValue = string.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings[Key] != null)
                KeyValue = System.Configuration.ConfigurationManager.AppSettings[Key].ToString();
            return KeyValue;
        }

        public static string ValidShortcutDocumentName(string DocumentName, string extension)
        {
            try
            {
                //string SupportedfileFormat = GetConfigKeyValue("SupportedfileFormat");
                if (!Path.GetExtension(DocumentName).Trim().Equals(extension))
                    DocumentName += "_shortcut";
                else
                    DocumentName = Path.GetFileNameWithoutExtension(DocumentName) + "_shortcut" + extension;

            }
            catch (Exception exp)
            {
                AggieGlobalLogManager.Fatal(exp, "ValidShortcutDocumentName");
            }
            return DocumentName;
        }

        public static string ValidDocumentName(string DocumentName, string extension)
        {

            try
            {
                string SupportedfileFormat = GetConfigKeyValue("SupportedfileFormat");
                string extensionInFileName = System.IO.Path.GetExtension(DocumentName).ToLower();

                if (extensionInFileName.Equals(string.Empty))
                {
                    return (DocumentName + extension);
                }
                if (!extensionInFileName.Equals(extension, StringComparison.OrdinalIgnoreCase))
                {
                    if (Array.IndexOf(SupportedfileFormat.Split(','), extensionInFileName) < 0)
                    {
                        return (DocumentName + extension);
                    }
                    return Path.GetFileNameWithoutExtension(DocumentName) + extension;
                }
            }
            catch (Exception exp)
            {
                AggieGlobalLogManager.Fatal(exp, "ValidDocumentName");
            }
            return DocumentName;
        }



       

        public static string FormatWith(this string format, object source)
        {
            return FormatWith(format, null, source);
        }

        public static string FormatWith(this string format, IFormatProvider provider, object source)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            Regex r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
              RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            List<object> values = new List<object>();
            string rewrittenFormat = r.Replace(format, delegate(Match m)
            {
                System.Text.RegularExpressions.Group startGroup = m.Groups["start"];
                System.Text.RegularExpressions.Group propertyGroup = m.Groups["property"];
                System.Text.RegularExpressions.Group formatGroup = m.Groups["format"];
                System.Text.RegularExpressions.Group endGroup = m.Groups["end"];

                values.Add((propertyGroup.Value == "0")
                  ? source
                  : System.Web.UI.DataBinder.Eval(source, propertyGroup.Value));

                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
                  + new string('}', endGroup.Captures.Count);
            });

            return string.Format(provider, rewrittenFormat, values.ToArray());
        }

        public static string GetStorageSize(this string FileSize)
        {
            string size = "0 KB";
            if (!string.IsNullOrEmpty(FileSize))
            {
                double mbitCount = default(double); // value;
                double.TryParse(FileSize, out mbitCount);

                if (mbitCount >= 1024)
                    size = String.Format("{0:##.#}", mbitCount / 1024) + " GB";
                if (mbitCount >= 1 && mbitCount < 1024)
                    size = String.Format("{0:##.#}", mbitCount) + " MB";
                if (mbitCount > 0 && mbitCount < 1)
                {
                    var kbitCount = mbitCount * 1024;
                    if (kbitCount >= 1)
                        size = String.Format("{0:##.#}", kbitCount) + " KB";
                    else if (kbitCount > 0 && kbitCount < 1)
                        size = "1 KB";
                }
                //if (mbyteCount > 1024)
            }
            return size;
        }

        public static string DownloadUrlPrefix
        {
            get;
            set;
        }

        public static HttpResponseMessage GetFileStream(string zipFilePath)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            MemoryStream memoryStream = new MemoryStream();

            if (File.Exists(zipFilePath))
            {
                FileStream file = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);

                byte[] bytes = new byte[file.Length];

                file.Read(bytes, 0, (int)file.Length);

                memoryStream.Write(bytes, 0, (int)file.Length);

                file.Close();

                httpResponseMessage.Content = new ByteArrayContent(memoryStream.ToArray());

                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                httpResponseMessage.StatusCode = HttpStatusCode.OK;
            }
            return httpResponseMessage;
        }

        public static string ReplaceSingleQuote(string inputValue, int numQuotes)
        {
            string toBePlacedBy = "";
            for (int loopIdx = 1; loopIdx <= numQuotes; loopIdx++)
            {
                toBePlacedBy += "'";
            }

            string retValue = inputValue.Replace("'", toBePlacedBy);
            return retValue;
        }
        #region MemoryCache

        /// <summary>
        /// Get value from MemoryCache
        /// </summary>
        /// <param name="key">Name of the key to fetch value from MemoryCache</param>
        /// <returns>value present in MemoryCache</returns>
        public static object GetValueFromMemoryCache(string key)
        {
            if ((!string.IsNullOrEmpty(key)) && HttpContext.Current != null)
            {
                MemoryCache memc = MemoryCache.Default;
                if (memc.Contains(key))
                    return memc.Get(key);
            }
            else
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// Add key value to MemoryCache 
        /// </summary>
        /// <param name="key">Name of the key</param>
        /// <param name="value">Value to put in cache</param>
        /// <param name="expirationDurationInSec">Time in second for cache expiration</param>
        /// <returns>Key value added success result</returns>
        public static bool AddToCache(string key, object value, double expirationDurationInSec)
        {

            if (value != null && HttpContext.Current != null)
            {
                MemoryCache memc = MemoryCache.Default;
                return memc.Add(key, value, DateTimeOffset.UtcNow.AddSeconds(expirationDurationInSec));
            }
            return false;
        }

        /// <summary>
        /// Deletes the Key value from the MemoryCache
        /// </summary>
        /// <param name="key">Name of the MemoryCache key to be deleted</param>
        /// <returns>success result</returns>
        public static bool DeleteMemoryCache(string key)
        {
            if ((!string.IsNullOrEmpty(key)) && HttpContext.Current != null)
            {
                MemoryCache memc = MemoryCache.Default;
                if (memc.Contains(key))
                {
                    memc.Remove(key);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region CACHE

        /// <summary>
        /// store data in application cache
        /// </summary>
        /// <param name="Key">name of the unique key</param>
        /// <param name="_Object">value to be stored</param>
        /// <param name="_CacheDuration">life span of the cache</param>
        public static void SetCache(string Key, Object _Object, CacheDuration _CacheDuration)
        {
            try
            {

                //if (Key != null && HttpContext.Current != null && _Object != null)
                //{
                //    if (_CacheDuration != CacheDuration.VeryHigh)
                //    {
                //        HttpContext.Current.Cache.Add(Key, _Object, null, DateTime.Now.AddSeconds(_CacheDuration.GetHashCode()), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                //    }
                //    else
                //    {
                //        HttpContext.Current.Cache.Add(Key, _Object, null, DateTime.Now.AddMinutes(_CacheDuration.GetHashCode()), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                //    }
                //}

            }

            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal(ex, "Utility:SetCache(): Failed to set cache for key={0} , data={1}", Key, _Object);
            }
        }

        /// <summary>
        /// get data from application cache
        /// </summary>
        /// <param name="Key">name of the cache key</param>
        /// <returns>object data stored in cache</returns>
        public static Object GetCache(string Key)
        {
            try
            {
                if (Key != null && HttpContext.Current != null && HttpContext.Current.Cache[Key] != null)
                {
                    return HttpContext.Current.Cache[Key] as Object;
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal(ex, "Utility:GetCache(): Failed to get cache for key={0} ", Key);
            }
            return null;
        }

        /// <summary>
        /// delete cache data from application cache
        /// </summary>
        /// <param name="Key">name of the cache key to be deleted</param>
        public static void DeleteCache(string Key)
        {
            try
            {
                if (!string.IsNullOrEmpty(Key) && HttpContext.Current != null && HttpContext.Current.Cache[Key] != null)
                {
                    HttpContext.Current.Cache.Remove(Key);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal(ex, "Utility:DeleteCache(): Failed to delete cache for key={0} ", Key);
            }
        }

        #region ClearCacheOnUpdate

        /// <summary>
        /// delete all request cycle related cache keys from application cache
        /// </summary>
        public static void ClearAllRequestCycleCache()
        {
            try
            {
                List<string> keys = new List<string>();
                keys.Add("RequestCycleCaching_");

                foreach (var key in keys)
                {
                    if (HttpContext.Current == null)
                        break;
                    foreach (var item in HttpContext.Current.Cache)
                    {
                        var k = (string)((System.Collections.DictionaryEntry)(item)).Key;
                        if (k.StartsWith(key))
                        {
                            DeleteCache(k);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal(ex, "Utility:ClearAllRequestCycleCache(): Failed :: ");
            }
        }

        #endregion

        #region ClearCacheOnAccountUpdate

        #endregion

        #region ClearCacheOnUserUpdate

        #endregion

        #endregion

        public static string BuildFilterClause(string format, string value)
        {
            string isScalable = HttpContext.Current.Session[AggieGlobal.WebApi.Controllers.Common.ApplicationConstant.DBIdentier] as string;
            return !string.IsNullOrWhiteSpace(value) ? string.Format(isScalable.ToLower() == "false" ? format : format.Replace("ESCAPE '\\'", "ESCAPE '\\\\'"), value.Replace("'", "''")) : string.Empty;
        }

        public static string BuildFilterClauseWithEscape(string format, string value)
        {
            string isScalable = HttpContext.Current.Session[AggieGlobal.WebApi.Controllers.Common.ApplicationConstant.DBIdentier] as string;
            return !string.IsNullOrWhiteSpace(value) ? string.Format(isScalable.ToLower() == "false" ? format : format.Replace("ESCAPE '\\'", "ESCAPE '\\\\'"), value.Replace("'", "''").Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[")) : string.Empty;
        }

        public static int ConvertToInt(this object Input)
        {
            int _retint = default(int);
            if (Input != null)
                _retint = (int)Input;

            return _retint;

        }

        public static bool ValidateStringWithRegex(string content, string RegexKey)
        {
            //var match = Regex.Match(content, RegexKey);
            var regex = new Regex(RegexKey);
            return regex.IsMatch(content);
        }

        
        public static object ProcessMillisecond(string source, string propertyName)
        {
            try
            {
                if (source != null && source.Length > 0)
                {
                    Newtonsoft.Json.Linq.JArray itmarr = Newtonsoft.Json.Linq.JArray.Parse(source);
                    foreach (var itm in itmarr)
                    {
                        var val = itm[propertyName];
                        if (val != null && val.ToString().Length > 0)
                        {
                            string[] tSplit = val.ToString().Split('.');
                            if (tSplit.Length == 2)
                            {
                                tSplit[1] = tSplit[1].Substring(6, tSplit[1].Length - 6);
                                itm[propertyName] = string.Join(".", tSplit);
                            }
                        }
                    }
                    return itmarr.ToString();
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("Millisecond conversion failed. exception{0}", ex.Message);
            }
            return source;
        }

        public static string KeywordSearch(string strKey)
        {
            char[] delimiterChars = { ' ', ';', ':' };
            //System.Console.WriteLine($"Original text: '{text}'");

            string[] words = strKey.Split(delimiterChars);
            //System.Console.WriteLine($"{words.Length} words in text:");

            string returnString = "[";
            bool isfield = true;
            int count = 0;
            int isAttachment = 0;
            int loopCount = 0;
            string searchVal = "";

            foreach (var word in words)
            {
                if (String.Equals(word.Trim().ToLower(), "to", StringComparison.OrdinalIgnoreCase))
                {
                    if (count > 0 && isAttachment != 1)
                        returnString = returnString + "'},";
                    else if (count > 0)
                        returnString = returnString + "},";

                    returnString = returnString + "{'Join':1,'FieldDataType':1,'ID':'txtMessageTo_filter'" + "," + "'SelectedValue':";
                    isfield = false;
                    count = 0;
                    isAttachment = 0;
                    loopCount++;
                }
                else if (String.Equals(word.Trim().ToLower(), "from", StringComparison.OrdinalIgnoreCase))
                {
                    if (count > 0 && isAttachment != 1)
                        returnString = returnString + "'},";
                    else if (count > 0)
                        returnString = returnString + "},";

                    returnString = returnString + "{'Join':1,'FieldDataType':1,'ID':'txtMessageFrom_filter'" + "," + "'SelectedValue':";
                    isfield = false;
                    count = 0;
                    isAttachment = 0;
                    loopCount++;
                }
                else if (String.Equals(word.Trim().ToLower(), "subject", StringComparison.OrdinalIgnoreCase))
                {
                    loopCount++;
                    if (count > 0 && isAttachment != 1)
                        returnString = returnString + "'},";
                    else if (count > 0)
                        returnString = returnString + "},";

                    returnString = returnString + "{'Join':1,'FieldDataType':1,'ID':'txtMessageSubject_filter'" + "," + "'SelectedValue':";
                    isfield = false;
                    count = 0;
                    isAttachment = 0;
                }
                else if (String.Equals(word.Trim().ToLower(), "date", StringComparison.OrdinalIgnoreCase))
                {
                    //if (count > 0)
                    //    returnString = returnString + "'},";

                    //returnString = returnString + "{'Join':1,'FieldDataType':3,'ID':'txtMessageDate_filter'" + "," + "'SelectedValue':";

                    isfield = false;
                    isAttachment = 0;
                    loopCount++;
                    //count = 0;
                }
                else if (String.Equals(word.Trim().ToLower(), "attachment", StringComparison.OrdinalIgnoreCase))
                {
                    if (count > 0)
                        returnString = returnString + "'},";

                    returnString = returnString + "{'Join':1,'FieldDataType':4,'ID':'isattachmentexist_filter'" + "," + "'SelectedValue':";
                    isAttachment = 1;
                    isfield = false;
                    count = 0;
                    loopCount++;
                }
                else
                {
                    isfield = true;
                }

                
            }

            if (count == 0)
            {
                return null;
            }

            returnString = isAttachment != 1 ? returnString + "'}]" : returnString + "}]";


            return returnString;
        }



    }


    /// <summary>
    /// assign life span of cache data
    /// </summary>
    public enum CacheDuration
    {
        /// <summary>
        /// value in seconds
        /// </summary>
        [Description("MicroCache")]
        MicroCache = 3,

        /// <summary>
        /// value in seconds
        /// </summary>
        [Description("Low")]
        LowCache = 5,

        /// <summary>
        /// value in seconds
        /// </summary>
        [Description("Medium")]
        Medium = 8,

        /// <summary>
        /// value in seconds
        /// </summary>
        [Description("High")]
        High = 10,

        /// <summary>
        /// value in minutes
        /// </summary>
        [Description("VeryHigh")]
        VeryHigh = 50,
    }
}