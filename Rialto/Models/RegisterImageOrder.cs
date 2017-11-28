using Rialto.Models.DAO.Builder;
using Rialto.Models.DAO.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public enum RegisterImageOrder
    {
        IdAsc,
        IdDesc,
        FileSizeAsc,
        FileSizeDesc,
        FileNameAsc,
        FileNameDesc
    }

    public static class RegisterImageOrderExt
    {
        public static OrderByItem ToOrderByItem(this RegisterImageOrder order)
        {
            switch (order)
            {
                case RegisterImageOrder.IdAsc: return REGISTER_IMAGE.ID.Asc();
                case RegisterImageOrder.IdDesc: return REGISTER_IMAGE.ID.Desc();
                case RegisterImageOrder.FileSizeAsc: return REGISTER_IMAGE.FILE_SIZE.Asc();
                case RegisterImageOrder.FileSizeDesc: return REGISTER_IMAGE.FILE_SIZE.Desc();
                case RegisterImageOrder.FileNameAsc: return REGISTER_IMAGE.FILE_NAME.Asc();
                case RegisterImageOrder.FileNameDesc: return REGISTER_IMAGE.FILE_NAME.Desc();

                default:
                    return REGISTER_IMAGE.ID.Desc();
            }
        }

        public static RegisterImageOrder Invert(this RegisterImageOrder order)
        {
            switch (order)
            {
                case RegisterImageOrder.IdAsc: return RegisterImageOrder.IdDesc;
                case RegisterImageOrder.IdDesc: return RegisterImageOrder.IdAsc;
                case RegisterImageOrder.FileSizeAsc: return RegisterImageOrder.FileSizeDesc;
                case RegisterImageOrder.FileSizeDesc: return RegisterImageOrder.FileSizeAsc;
                case RegisterImageOrder.FileNameAsc: return RegisterImageOrder.FileNameDesc;
                case RegisterImageOrder.FileNameDesc: return RegisterImageOrder.FileNameAsc;

                default:
                    throw new Exception();
            }
        }
    }
}
