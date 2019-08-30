using AggieGlobal.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.WebApi.Models.Public
{
    public class PasswordHistory : ModelBase
    {
        public int PasswordHistoryID { get; set; }
        public int PWUserID { get; set; }
        public string Password { get; set; }
        public int SlNo { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}