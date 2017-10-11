using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DataModel
{
    public class ThumbnailImageSize
    {
        public string DispThumbnailSize
        {
            get
            {
                switch (this.ThumbnailSize)
                {
                    case Size.Large:
                        return "大";
                    case Size.Middle:
                        return "中";
                    case Size.Small:
                        return "小";
                    default:
                        return string.Empty;
                }
            }
        }
        
        public Size ThumbnailSize { get; set; }
        public enum Size {
            Large,
            Middle,
            Small
        }
    }
}
