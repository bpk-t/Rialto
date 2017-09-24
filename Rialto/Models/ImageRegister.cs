using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;
using Rialto.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Rialto.Models
{
    public static class ImageRegister
    {
        /// <summary>
        /// 画像ファイルを登録する
        /// </summary>
        /// <param name="fileList">登録する画像ファイル、または画像ファイルが格納されたディレクトリ</param>
        public static IEnumerable<LangExt.Option<long>> RegistImages(string[] fileList)
        {
            var destDir = MakeTodayDir();
            var tasks = Flat(fileList)
                .Where(f => IsImageExt(f))
                .Where(f => ExistsFile(f, destDir))
                .Where(f => ExistsDB(f))
                .Do(f => CopyFile(f, destDir))
                .Select(f => new ImageRegisterTask(f));

            return WorkerTaskExecutor.Instance.Execute(new Transaction<ImageRegisterTask>(tasks))
                .Tasks.Select(t => t.IMGINF_ID);
        }

        private static List<FileInfo> Flat(string [] fileList)
        {
            return _Flat(fileList, 0, new List<FileInfo>());
        }

        private static List<FileInfo> _Flat(string[] fileList, int index, List<FileInfo> resultList)
        {
            if ((fileList.Length - 1) <= index)
            {
                return resultList;
            }
            var filePath = fileList[index];
            if (Directory.Exists(filePath))
            {
                _Flat(Directory.GetFiles(filePath), 0, resultList);
            }
            else if (File.Exists(filePath))
            {
                resultList.Add(new FileInfo(filePath));
            }
            return _Flat(fileList, index + 1, resultList);
        }

        private static bool ExistsFile(FileInfo file, string destDir)
        {
            var destFileName = Path.Combine(destDir, file.Name);
            return File.Exists(destFileName);
        }

        private static bool ExistsDB(FileInfo file)
        {
            var hashValue = MD5Helper.GenerateMD5HashCodeFromFile(file.FullName);
            return M_IMAGE_INFORepository.FindByHash(hashValue).IsSome;
        }

        private static FileInfo CopyFile(FileInfo file, string destDir)
        {
            var destFileName = Path.Combine(destDir, file.Name);
            File.Copy(file.FullName, destFileName);
            return new FileInfo(destFileName);
        }

        /// <summary>
        /// 今日の日付のディレクトリを作成する
        /// </summary>
        /// <returns></returns>
        private static string MakeTodayDir()
        {
            var imgDataDir = Path.Combine(Properties.Settings.Default.ImgDataDirectory, DateTime.Now.ToString("yyyyMMdd"));
            //今日の日付のフォルダがない場合は作成する
            if (!Directory.Exists(imgDataDir))
            {
                Directory.CreateDirectory(imgDataDir);
            }
            return imgDataDir;
        }

        /// <summary>
        /// 拡張子が画像のファイルの場合、trueを返す
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static bool IsImageExt(FileInfo file)
        {
            return file.Extension == ".jpg"
                || file.Extension == ".jpeg"
                || file.Extension == ".gif"
                || file.Extension == ".png"
                || file.Extension == ".bmp";
        }
    }
}
