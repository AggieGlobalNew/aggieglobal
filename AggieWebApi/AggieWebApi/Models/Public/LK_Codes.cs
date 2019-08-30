using AggieGlobal.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.WebApi.Models.Public
{
    public class LK_Codes: ModelBase
    {
        public int CodeID { get; set; }
        public string CodeIdentifier { get; set; }
        public string CodeDesc { get; set; }
        public int OrdinalNum { get; set; }
    }
}