using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public abstract class Worker
    {
        /// <summary>
        /// Worker Id
        /// </summary>
        private string id = Guid.NewGuid().ToString();

        /// <summary>
        /// WorkerID
        /// </summary>
        public string ID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <returns>実行結果</returns>
        public abstract bool Execute();

        /// <summary>
        /// 元に戻す
        /// </summary>
        /// <returns></returns>
        public abstract bool Rollback();
    }
}
