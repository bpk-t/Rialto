using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Media.Imaging;

namespace Rialto.Models
{
    public class ThumbnailImage
    {
        /// <summary>
        /// サムネイル画像を返す
        /// </summary>
        /// <param name="imgPath">元画像のファイルパス</param>
        /// <param name="imgHash">元画像のハッシュ</param>
        /// <returns></returns>
        public Uri GetThumbnailImage(string imgPath, string imgHash)
        {
            var filePath = GetPath(imgHash);
            if (!File.Exists(filePath))
            {
                //CreateThumbnailImage(imgPath, imgHash);
            }
            return new Uri(filePath);
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(Properties.Settings.Default.ThumbnailImageDirectory, fileName);
        }

        /// <summary>
        /// サムネイル画像のディレクトリを作成する
        /// </summary>
        private void MakeThumbDir()
        {
            if (!Directory.Exists(Properties.Settings.Default.ThumbnailImageDirectory))
            {
                FileSystem.MkDir(Properties.Settings.Default.ThumbnailImageDirectory);
            }
        }
    }
}
