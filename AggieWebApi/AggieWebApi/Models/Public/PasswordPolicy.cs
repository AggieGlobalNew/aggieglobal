using AggieGlobal.Models.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace AggieGlobal.WebApi.Models.Public
{
    public class PasswordPolicy : ModelBase
    {      
        public int PolicyID { get; set; }
        public int PWAccountID { get; set; }
        public int CreateBy { get; set; }
        public int ModifyBy { get; set; }
        public int ExpiryDays { get; set; }
        
        public string Policy { get; set; }
     
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsTwoStepEnabled { get; set; }
        public PasswordComplexity PasswordComplexitydetails { get; set; }
        public DateTime PasswordComplexityDate { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public string PasswordComplexity { get; set; }
    }

    public class PasswordComplexity
    {
        public int Minlength { get; set; }
        public int Maxlength { get; set; }
        public int Blockrecent { get; set; }
        public int MinNumeric { get; set; }
        public int MinSplCharacter { get; set; }
        public int MinNumOrSplChar { get; set; }
        public int MinCapitalcaseletters { get; set; }
        public string AllowedSplcharacters { get; set; }
        public string Regex { get; set; }
    }

    [Serializable]
    public class UserSecurityQuestion
    {
        private int _codeid = 0;
        public int CodeID { get { return _codeid; } set { _codeid = value; } }

        private string _answer = string.Empty;
        public string Answer { get { return _answer; } set { _answer = value; } }
    }
}