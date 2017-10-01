using LangExt;
using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;
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
        public Option<long> IMGINF_ID { get; set; }

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
        private Option<long> InsertImageFromFile(FileInfo file)
        {
            /*
            var img = new System.Drawing.Bitmap(file.FullName);
            var inserted = M_IMAGE_INFORepository.Insert(new M_IMAGE_INFO()
            {
                FILE_SIZE = (int)file.Length,
                FILE_NAME = Path.GetFileNameWithoutExtension(file.Name),
                FILE_TYPE = file.Extension.Substring(1),
                FILE_PATH = file.FullName,
                HASH_VALUE = MD5Helper.GenerateMD5HashCodeFromFile(file.FullName),
                HEIGHT_PIX = img.Height,
                WIDTH_PIX = img.Width,
                COLOR = 0,
                DO_GET = 2,
                DELETE_FLG = 0,
                DELETE_REASON_ID = 0,
                DELETE_DATE = DBHelper.DATETIME_DEFAULT_VALUE
            });
            AverageHashGenerator.Insert(inserted.IMGINF_ID.Value);
            return Option.Create(inserted.IMGINF_ID);
            */
            return Option.None;
        }
    }
}
