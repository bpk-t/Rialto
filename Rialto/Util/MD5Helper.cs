using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Util
{
    public class MD5Helper
    {
        /// <summary>
        /// ファイルからMD5ハッシュ文字列を返す
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GenerateMD5HashCodeFromFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var md5Provider = new MD5CryptoServiceProvider();
                var bs = md5Provider.ComputeHash(fs);
                md5Provider.Clear();
                return bs.Aggregate(string.Empty, (acc, b) => acc + b.ToString("x2"));
            }
        }
    }
}
