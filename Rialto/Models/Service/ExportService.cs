using Rialto.Constant;
using Rialto.Models.DAO.Builder;
using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;
using Rialto.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using NLog;

namespace Rialto.Models.Service
{
    public class ExportOptions
    {
        /// <summary>
        /// 表示順になるようにファイル名を変更する
        /// </summary>
        public bool OrderRename { get; set; }
    }

    public class ExportService
    {
        private static readonly Logger logger = LogManager.GetLogger("fileLogger");

        public Task<List<Try<string>>> Export(string exportDir, long tagId, Option<ExportOptions> options)
        {
            if (Directory.Exists(exportDir))
            {
                return DBHelper.Instance.Execute((connection, tran) => {

                    return (from list in GetAllImage(connection, tagId)
                            from destList in CopyFiles(list, exportDir)
                            select destList);
                });
            } else
            {
                return Task.FromResult(new System.Collections.Generic.List<Try<string>>());
            }
        }

        private Task<List<Try<string>>> CopyFiles(IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)> list, string exportDir)
        {
            return Task.Run(() => {
                return list.Select(item =>
                    (from registerImage in item.Item1
                     from repository in item.Item2
                     select (Path.Combine(repository.Path, registerImage.FilePath), $"{registerImage.FileName}.{registerImage.FileExtension}"))
                         .Fold(Try<string>(new Exception("Option None")), (acc, x) =>
                         {
                             (string sourcePath, string fileName) = x;
                             try
                             {
                                 var destFilePath = Path.Combine(exportDir, fileName);
                                 if (File.Exists(destFilePath))
                                 {
                                    // スキップ
                                    logger.Debug($"Export Skip = {destFilePath}");
                                 }
                                 else
                                 {
                                     File.Copy(sourcePath, destFilePath);
                                 }

                                 return Try(destFilePath);
                             }
                             catch (Exception e)
                             {
                                 return Try<string>(e);
                             }
                         })
                    ).ToList();
            });
        }
        private Task<IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)>> GetAllImage(DbConnection connection, long tagId)
        {
            if (tagId == TagConstant.ALL_TAG_ID)
            {
                return RegisterImageRepository.GetAllAsync(connection, None, None, Order.Asc);
            }
            else if (tagId == TagConstant.NOTAG_TAG_ID)
            {
                return RegisterImageRepository.GetNoTagAsync(connection, None, None, Order.Asc);
            }
            else
            {
                return RegisterImageRepository.GetByTagAsync(connection, tagId, None, None, Order.Asc);
            }
        }
    }
}
