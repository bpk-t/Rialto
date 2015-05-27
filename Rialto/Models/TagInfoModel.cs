using Rialto.Util;
using Rialto.ViewModels.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public class TagInfoModel
    {
        private DBHelper db = DBHelper.GetInstance();

        /// <summary>
        /// 全てのタグ情報を返す
        /// </summary>
        /// <returns></returns>
        public List<TagMasterInfo> GetAllTag()
        {
            var result = new List<TagMasterInfo>();

            return result;
        }
    }
}
