using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using LanguageExt;
using static LanguageExt.Prelude;
using System.IO;
using Rialto.Util;
using Rialto.Models.Repository;
using Rialto.Models.DAO.Entity;
using System.Data.Common;

namespace Rialto.Models.Service
{
    public class ImageRegisterService
    {
        public class RegisterFailureInfo
        {
            public string Path { get; }
            public string Result { get; }
            public RegisterFailureInfo(string path, string result)
            {
                Path = path;
                Result = result;
            }
        }

        private IActorRef registerActor;
        public ImageRegisterService(ActorSystem system)
        {
            registerActor = system.ActorOf<ImageRegisterActor>(nameof(ImageRegisterActor));
        }

        public Task<List<Either<RegisterFailureInfo, FileInfo>>> RegisterImages(IList<Uri> imageUris, Option<int> tagId)
        {
            var message = new ImageRegisterActor.RegisterImagesMessage(imageUris, tagId);
            return registerActor.Ask<List<Either<RegisterFailureInfo, FileInfo>>>(message);
        }

        class ImageRegisterActor : ReceiveActor
        {
            public class RegisterImagesMessage
            {
                public IList<Uri> ImageUris { get; }
                public Option<int> TagId { get; }
                public RegisterImagesMessage(IList<Uri> imageUris, Option<int> tagId)
                {
                    ImageUris = imageUris;
                    TagId = tagId;
                }
            }
            public ImageRegisterActor()
            {
                Receive<RegisterImagesMessage>((message) =>
                {
                    Sender.Tell(RegisterImages(message.ImageUris.Select(x => x.AbsolutePath).ToArray(), message.TagId));
                });
            }

            /// <summary>
            /// 画像ファイルを登録する
            /// </summary>
            /// <param name="fileList">登録する画像ファイル、または画像ファイルが格納されたディレクトリ</param>
            public Task<IEnumerable<Either<RegisterFailureInfo, FileInfo>>> RegisterImages(string[] fileList, Option<int> tagId)
            {
                return DBHelper.Execute((connection, tran) =>
                {
                    var destDir = MakeTodayDir();
                    var tasks = TreeToList(fileList)
                        .Select(x => x.Bind(f =>
                        {
                            if (IsImageExt(f))
                            {
                                return Right<RegisterFailureInfo, FileInfo>(f);
                            }
                            else
                            {
                                return Left<RegisterFailureInfo, FileInfo>(new RegisterFailureInfo(path: f.FullName, result: "画像形式ではないファイル"));
                            }
                        }))
                        .Select(x => x.Bind(f =>
                        {
                            if (ExistsFile(f, destDir))
                            {
                                return Left<RegisterFailureInfo, FileInfo>(new RegisterFailureInfo(path: f.FullName, result: "同名のファイルが既に存在します"));
                            }
                            else
                            {
                                return Right<RegisterFailureInfo, FileInfo>(f);
                            }
                        }))
                        .Select(x => x.Match(
                            Right: (f) => ExistsDB(connection, f).Select(result =>
                            {
                                if (result)
                                {
                                    return Left<RegisterFailureInfo, FileInfo>(new RegisterFailureInfo(path: f.FullName, result: "同じMD5ハッシュを持つ画像がDBに存在します"));
                                }
                                else
                                {
                                    return Right<RegisterFailureInfo, FileInfo>(f);
                                }
                            }),
                            Left: (l) => Task.FromResult(Left<RegisterFailureInfo, FileInfo>(l))
                         ))
                    .Select(t => t.Select(e => e.Bind(x => CopyFile(x, destDir))))
                    .Select(t => t.SelectMany(e => e.Match(
                            Right: (f) =>
                            {
                                try
                                {
                                    var img = new System.Drawing.Bitmap(f.FullName);
                                    var r = new RegisterImage
                                    {
                                        FileSize = (int)f.Length, // TODO
                                            FileName = Path.GetFileNameWithoutExtension(f.Name),
                                        FileExtension = f.Extension.Substring(1),
                                        FilePath = Path.Combine(destDir, f.Name),
                                        Md5Hash = MD5Helper.GenerateMD5HashCodeFromFile(f.FullName), // TODO 効率化
                                            AveHash = AverageHashGenerator.ComputeAveHash(f.FullName),
                                        HeightPix = img.Height,
                                        WidthPix = img.Width,
                                        DoGet = 2,
                                        DeleteTimestamp = null
                                    };

                                    return RegisterImageRepository.InsertAsync(connection, r).Select(nouse => Right<RegisterFailureInfo, FileInfo>(f));
                                }
                                catch (Exception exception)
                                {
                                    var left = Left<RegisterFailureInfo, FileInfo>(new RegisterFailureInfo(path: f.FullName, result: $"画像登録時に何らかのエラーが発生しました。message={exception.Message}"));
                                    return Task.FromResult(left);
                                }
                            },
                            Left: (l) => Task.FromResult(Left<RegisterFailureInfo, FileInfo>(l))
                            )));

                    return Task.WhenAll(tasks)
                        .ContinueWith(nouse => tasks.Select(t => t.Result));
                });
            }

            private IEnumerable<Either<RegisterFailureInfo, FileInfo>> TreeToList(string[] fileList)
            {
                List<Either<RegisterFailureInfo, FileInfo>> _TreeToList(string[] filePathList, List<Either<RegisterFailureInfo, FileInfo>> resultList)
                {
                    foreach (var filePath in filePathList)
                    {
                        if (Directory.Exists(filePath))
                        {
                            _TreeToList(Directory.GetFiles(filePath), resultList);
                        }
                        else if (File.Exists(filePath))
                        {

                            resultList.Add(Right<RegisterFailureInfo, FileInfo>(new FileInfo(filePath)));
                        }
                        else
                        {
                            resultList.Add(Left<RegisterFailureInfo, FileInfo>(new RegisterFailureInfo(path: filePath, result: "存在しないファイル")));
                        }
                    }
                    return resultList;
                }
                return _TreeToList(fileList, new List<Either<RegisterFailureInfo, FileInfo>>());
            }

            private bool ExistsFile(FileInfo file, string destDir)
            {
                var destFileName = Path.Combine(destDir, file.Name);
                return File.Exists(destFileName);
            }

            private Task<bool> ExistsDB(DbConnection connection, FileInfo file)
            {
                var hashValue = MD5Helper.GenerateMD5HashCodeFromFile(file.FullName);
                return RegisterImageRepository.FindByHashAsync(connection, hashValue).Select(x => x.IsSome);
            }

            private Either<RegisterFailureInfo, FileInfo> CopyFile(FileInfo file, string destDir)
            {
                try
                {
                    var destFileName = Path.Combine(destDir, file.Name);
                    File.Copy(file.FullName, destFileName);
                    return Right<RegisterFailureInfo, FileInfo>(new FileInfo(destFileName));
                } catch (Exception e)
                {
                    return Left<RegisterFailureInfo, FileInfo>(new RegisterFailureInfo(path: file.FullName, result: $"ファイルのコピーに失敗しました。message={e.Message}"));
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
            /// 拡張子が画像のファイルの場合、trueを返す
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            private bool IsImageExt(FileInfo file)
            {
                return file.Extension == ".jpg"
                    || file.Extension == ".jpeg"
                    || file.Extension == ".gif"
                    || file.Extension == ".png"
                    || file.Extension == ".bmp";
            }
        }
    }
}
