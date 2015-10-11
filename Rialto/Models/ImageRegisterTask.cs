using Rialto.Models.DAO;
using Rialto.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public class ImageRegisterTask : IWorkerTask
    {
        public ImageRegisterTask(FileInfo targetFile)
        {
            TargetFile = targetFile;
        }

        public FileInfo TargetFile { get; set; }
        public long? IMGINF_ID { get; set; }

        public void Execute()
        {
            IMGINF_ID = InsertImageFromFile(TargetFile);
        }

        public void Rollback()
        {
            // TODO 未実装
        }

        /// <summary>
        /// ファイルをDBに登録する
        /// </summary>
        /// <param name="file">登録するファイル情報</param>
        /// <returns>登録した画像情報IDを返す、既にDBに存在している場合はその画像情報IDを返す、エラーの場合は-1</returns>
        private long? InsertImageFromFile(FileInfo file)
        {
            var hashValue = MD5Helper.GenerateMD5HashCodeFromFile(file.FullName);
            var img = new System.Drawing.Bitmap(file.FullName);
            var insertObj = new M_IMAGE_INFO()
            {
                FILE_SIZE = (int)file.Length,
                FILE_NAME = Path.GetFileNameWithoutExtension(file.Name),
                FILE_TYPE = file.Extension.Substring(1),
                FILE_PATH = file.FullName,
                HASH_VALUE = hashValue,
                HEIGHT_PIX = img.Height,
                WIDTH_PIX = img.Width,
                COLOR = 0,
                DO_GET = 2,
                DELETE_FLG = 0,
                DELETE_REASON_ID = 0,
                DELETE_DATE = DBHelper.DATETIME_DEFAULT_VALUE
            };

            var inserted = M_IMAGE_INFO.Insert(insertObj);
            var registor = new AverageHashGenerator(inserted.IMGINF_ID.Value);
            registor.Insert();
            return inserted.IMGINF_ID;
        }
    }
}
