﻿using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    // TODO クラス名変更
    public class T_ADD_TAG_DEF : TableDefinition
    {
        private T_ADD_TAG_DEF() : base("T_ADD_TAG") {}

        public static T_ADD_TAG_DEF ThisTable = new T_ADD_TAG_DEF();

        public static ColumnDefinition IMGINF_ID = new ColumnDefinition(ThisTable, nameof(IMGINF_ID));
        public static ColumnDefinition TAGINF_ID = new ColumnDefinition(ThisTable, nameof(TAGINF_ID));
        public static ColumnDefinition CREATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(CREATE_LINE_DATE));
        public static ColumnDefinition UPDATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(UPDATE_LINE_DATE));
    }
}
