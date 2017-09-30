﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Entity
{
    public class ImageScore
    {
        public int RegisterImageId { get; set; }
        public int TagId { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}