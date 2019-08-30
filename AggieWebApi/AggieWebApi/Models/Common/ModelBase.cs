using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.Models.Common
{
    public class ModelBase
    {
        public ModelBase()
        {

        }
        public ResponseStatus Status
        {
            get;
            set;//made public to allow setting this property from framework (data from memcache deserialization)
        }

        public string Error
        {
            get;
            set;
        }


    }

    public enum ResponseStatus : int
    {
        Unknown = 0,
        Successful = 1,
        Failed = 2,
        noPermission = 3,
        DuplicateData = 4
    }




    public class SessionData
    {
        internal int _userId { get; set; }
        internal string _email { get; set; }
        internal string _deviceid { get; set; }
        //Private Constructor.  
        public SessionData()
        {
        }
    }
}