using System;
using System.Collections.Generic;
using System.Linq;
using Rialto.Util;
using Dapper;

namespace Rialto.Models.DAO.Entity
{
    public class M_TAGADDTAB
    {
        public long? TABSET_ID { get; set; }
        public string TAB_NAME { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }
    }
}
