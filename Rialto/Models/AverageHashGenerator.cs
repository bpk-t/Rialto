using Rialto.Models.DAO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public static class AverageHashGenerator
    {
        private static readonly int PIC_SIZE = 16;

        public static void Insert(long IMGINF_ID)
        {
            M_IMAGE_INFO.FindById(IMGINF_ID).Match(
                (some) =>
                {
                    T_AVEHASH.Insert(new T_AVEHASH()
                    {
                        IMGINF_ID = IMGINF_ID,
                        AVEHASH = ComputeAveHash(some.FILE_PATH)
                    });
                },
                () => { });
        }

        private static string ComputeAveHash(string imgPath)
        {
            return GetHashStr(
                    GetGlayscaleBitmap(
                        ReduceBitmap(new Bitmap(imgPath), PIC_SIZE), PIC_SIZE));
        }

        /// <summary>
        /// 指定されたサイズの正方形に画像を縮小する
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static Bitmap ReduceBitmap(Bitmap srcBmp, int size)
        {
            var reduceBmp = new Bitmap(size, size);
            Graphics g = Graphics.FromImage(reduceBmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            g.DrawImage(srcBmp, 0, 0, size, size);
            return reduceBmp;
        }

        /// <summary>
        /// グレースケールに変換したBitmapを返す
        /// </summary>
        /// <param name="srcBitmap"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static int[] GetGlayscaleBitmap(Bitmap srcBitmap, int size)
        {
            var result = new int[size * size];
            int count = 0;
            for (int i = 0; i < srcBitmap.Width; ++i)
            {
                for (int j = 0; j < srcBitmap.Height; ++j)
                {
                    var c = srcBitmap.GetPixel(i, j);
                    result[count] = (c.R + c.G + c.B) / 3;
                    count++;
                }
            }
            return result;
        }

        /// <summary>
        /// ハッシュ文字列を返す
        /// </summary>
        /// <param name="intArray"></param>
        /// <returns></returns>
        private static string GetHashStr(int[] intArray)
        {
            var ave = intArray.Average();

            int hash = 0;
            int bit = 0;
            var hashStr = new StringBuilder();
            foreach (var p in intArray)
            {
                hash |= (p > ave ? 1 : 0) << bit;
                if (bit == 7)
                {
                    hashStr.Append(((byte)(hash & 0xFF)).ToString("X2"));
                    hash = 0;
                    bit = 0;
                }
                else
                {
                    bit++;
                }
            }
            return hashStr.ToString();
        }
    }
}
