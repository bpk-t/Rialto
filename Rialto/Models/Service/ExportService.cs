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

namespace Rialto.Models.Service
{
    public class ExportService
    {
        public Task<IEnumerable<Try<string>>> Export(string exportDir, long tagId)
        {
            if (Directory.Exists(exportDir))
            {
                return DBHelper.Instance.Execute((connection, tran) => {
                    return GetAllImage(connection, tagId).Select(list =>
                    {
                        return list.Select(item =>
                        (from registerImage in item.Item1
                         from repository in item.Item2
                         select (Path.Combine(repository.Path, registerImage.FilePath), registerImage.FileName))
                             .Fold(Try<string>(new Exception("Option None")), (acc, x) => {
                                 (string sourcePath, string fileName) = x;
                                 try
                                 {
                                     var destFilePath = Path.Combine(exportDir, fileName);
                                     File.Copy(sourcePath, destFilePath);
                                     return Try(destFilePath);
                                 }
                                 catch (Exception e)
                                 {
                                     return Try<string>(e);
                                 }
                             })
                        );
                    });
                });
            } else
            {
                return Task.FromResult(Enumerable.Empty<Try<string>>());
            }
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
