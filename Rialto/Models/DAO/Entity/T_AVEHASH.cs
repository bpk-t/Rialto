using Rialto.Util;
using System;
using System.Data.SQLite;
using Dapper;

namespace Rialto.Models.DAO.Entity
{
    public class T_AVEHASH
    {
        public long? IMGINF_ID { get; set; }
        public string AVEHASH { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }
    }
}
