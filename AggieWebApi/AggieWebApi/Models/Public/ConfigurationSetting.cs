using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.WebApi.Models.Public
{
    public class ConfigurationSetting
    {
        public string ConfigKey {get;set;}
        public string ConfigValue {get;set;}
    }
    public enum EnvironmentType
    {
        None=0,
        US=1,
        UK=2
    }
}