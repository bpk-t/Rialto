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
                            from destList in CopyFiles(list, exportDir, options)
                            select destList);
                });
            } else
            {
                return Task.FromResult(new System.Collections.Generic.List<Try<string>>());
            }
        }

        private Task<List<Try<string>>> CopyFiles(IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)> list, string exportDir, Option<ExportOptions> options)
        {
            return Task.Run(() => {
                return list.Select((item, index) =>
                    (from registerImage in item.Item1
                     from repository in item.Item2
                     select (Path.Combine(repository.Path, registerImage.FilePath), registerImage.FileName, registerImage.FileExtension)
                     )
                         .Fold(Try<string>(new Exception("Option None")), (acc, x) =>
                         {
                             (string sourcePath, string fileName, string fileExtension) = x;
                             try
                             {
                                 var destFilePath = options.Bind(y => y.OrderRename ? Some(y) : None).Fold(
                                     Path.Combine(exportDir, $"{fileName}.{fileExtension}"),
                                     (a, y) => Path.Combine(exportDir, string.Format("{0:000000}.{1}", index, fileExtension)));

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
            // TODO Optionから取得する
            var order = RegisterImageOrder.IdDesc;
            if (tagId == TagConstant.ALL_TAG_ID)
            {
                return RegisterImageRepository.GetAllAsync(connection, None, None, order.ToOrderByItem());
            }
            else if (tagId == TagConstant.NOTAG_TAG_ID)
            {
                return RegisterImageRepository.GetNoTagAsync(connection, None, None, order.ToOrderByItem());
            }
            else
            {
                return RegisterImageRepository.GetByTagAsync(connection, tagId, None, None, order.ToOrderByItem());
            }
        }
    }
}
