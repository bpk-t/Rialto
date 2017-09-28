using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DataModel
{
    public class PageViewImageCount
    {
        public string DispImageCount { get; set; }
        public int ImageCount
        {
            get
            {
                return int.Parse(DispImageCount);
            }
        }
    }
}
