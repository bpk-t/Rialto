﻿using System;

namespace Rialto.Models.DAO.Entity
{
    public class M_TABSETTING
    {
        public long? TABSET_ID { get; set; }
        public long TAGINF_ID { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }
    }
}