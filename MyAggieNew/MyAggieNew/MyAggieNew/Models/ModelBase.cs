using System;

namespace MyAggieNew
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

        protected DateTime? _FarmEstablishedDate = null;
    }

    public enum ResponseStatus : int
    {
        Unknown = 0,
        Successful = 1,
        Failed = 2,
        noPermission = 3,
        DuplicateData = 4
    }
}