using Rialto.Models.DAO;
using Rialto.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Rialto.Models
{
    public class ImageRegister
    {
        /// <summary>
        /// 画像ファイルを登録する
        /// </summary>
        /// <param name="fileList">登録する画像ファイル、または画像ファイルが格納されたディレクトリ</param>
        public Tuple<List<long>,int> RegistImages(string[] fileList)
        {
            var notRegistCount = 0;
            var registIdList = CopyDataDir(fileList, MakeTodayDir())
                .Aggregate(new List<long>(), (acc, x) => 
                {
                    x.Match(
                        (success) =>
                        {
                            if (File.Exists(success))
                            {
                                acc.Add(InsertImageFromFile(new FileInfo(success)));
                            }
                            else if (Directory.Exists(success))
                            {
                                acc.AddRange(RegistAllImageFromDir(success));
                            }
                        },
                        (failure) => { notRegistCount++; });
                    return acc;
                });
            return Tuple.Create(registIdList, notRegistCount);
        }

        public IEnumerable<string> Flat(string [] fileList)
        {
            return fileList.Where(x => Directory.Exists(x))
                .Select(x => Directory.GetFiles(x))
                .SelectMany(x => Flat(x));
        }

        /// <summary>
        /// データフォルダにコピーする
        /// </summary>
        /// <param name="targetFiles"></param>
        /// <param name="imgDataDir"></param>
        /// <returns></returns>
        private IEnumerable<LangExt.Result<string, string>> CopyDataDir(string[] targetFiles, string imgDataDir)
        {
            return targetFiles.Select(dragFile => 
                    (File.Exists(dragFile)) ? 
                        CopyFile(dragFile, imgDataDir) : 
                        CopyDirectory(dragFile, imgDataDir));
        }

        private LangExt.Result<string, string> CopyFile(string srcFile, string destDir)
        {
            var file = new FileInfo(srcFile);
            var destFileName = Path.Combine(destDir, file.Name);
            if (File.Exists(destFileName))
            {
                return LangExt.Result.Failure(srcFile);
            }
            else
            {
                File.Copy(srcFile, destFileName);
                return LangExt.Result.Success(destFileName);
            }
        }

        private LangExt.Result<string, string> CopyDirectory(string srcDir, string destDir)
        {
            var dir = new DirectoryInfo(srcDir);
            var destDirName = Path.Combine(destDir, dir.Name);
            if (Directory.Exists(destDirName))
            {
                return LangExt.Result.Failure(srcDir);
            }
            else
            {
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(srcDir, destDirName);
                return LangExt.Result.Success(destDirName);
            }
        }

        /// <summary>
        /// 今日の日付のディレクトリを作成する
        /// </summary>
        /// <returns></returns>
        private string MakeTodayDir()
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
        /// 対象のフォルダ配下にある画像をDBに登録する
        /// </summary>
        /// <param name="targetDir"></param>
        private IEnumerable<long> RegistAllImageFromDir(string targetDir)
        {
            return (new DirectoryInfo(targetDir)).GetFiles().Select(fi => InsertImageFromFile(fi));
        }

        /// <summary>
        /// ファイルをDBに登録する
        /// </summary>
        /// <param name="fi">登録するファイル情報</param>
        /// <returns>登録した画像情報IDを返す、既にDBに存在している場合はその画像情報IDを返す、エラーの場合は-1</returns>
        private long InsertImageFromFile(FileInfo fi)
        {
            //ファイルが画像ファイルではない場合
            if (!IsImageExt(fi)) { return -1;}

            //ファイルが既にDBに存在する場合は登録しない
            var hashValue = MD5Helper.GenerateMD5HashCodeFromFile(fi.FullName);
            return M_IMAGE_INFO.FindByHash(hashValue).GetOrElse(() => {
                var img = new System.Drawing.Bitmap(fi.FullName);
                var insertObj = new M_IMAGE_INFO()
                {
                    FILE_SIZE = (int)fi.Length,
                    FILE_NAME = Path.GetFileNameWithoutExtension(fi.Name),
                    FILE_TYPE = fi.Extension.Substring(1),
                    FILE_PATH = fi.FullName,
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
                return inserted;
            }).IMGINF_ID.Value;
        }

        /// <summary>
        /// 拡張子が画像のファイルの場合、trueを返す
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        private bool IsImageExt(FileInfo fi)
        {
            return fi.Extension == ".jpg"
                || fi.Extension == ".jpeg"
                || fi.Extension == ".gif"
                || fi.Extension == ".png"
                || fi.Extension == ".bmp";
        }
    }
}
