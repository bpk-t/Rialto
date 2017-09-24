using Dapper;
using Rialto.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Entity
{
    public class T_ADD_TAG
    {
        public long? IMGINF_ID { get; set; }
        public long? TAGINF_ID { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }
    }
}
