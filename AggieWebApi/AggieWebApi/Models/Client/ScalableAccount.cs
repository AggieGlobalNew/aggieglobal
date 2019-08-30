using AggieGlobal.Models.Common;
using System;

namespace AggieGlobal.WebApi.Models.Client
{
    public class ScalableAccount : ModelBase
    {
        public int PWAccountID { get; set; }
        public int? PWUserID { get; set; }
        public string LoginID { get; set; }
        public bool IsScalable { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsMigrated { get; set; }
        public string Email { get; set; }
        public int? LoginTokenID { get; set; }
    }

    public class ScalableProject : ModelBase
    {
        public int PINProjectID { get; set; }
        public int PWAccountID { get; set; }
        public bool IsMigrated { get; set; }
        public DateTime CreateDate { get; set; }
    }
}