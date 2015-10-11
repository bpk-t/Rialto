using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public class WorkerTaskExecutor
    {
        private static readonly Lazy<WorkerTaskExecutor> _Instance =
            new Lazy<WorkerTaskExecutor>(() => new WorkerTaskExecutor());

        public static WorkerTaskExecutor Instance
        {
            get
            {
                return _Instance.Value;
            }
        }

        private IWorkerTask prevTask = null;

        public IWorkerTask Execute(IWorkerTask task)
        {
            task.Execute();
            prevTask = task;
            return task;
        }

        public IWorkerTask Rollback()
        {
            var ret = prevTask;
            if (ExistsPrevTask())
            {
                prevTask.Rollback();
                prevTask = null;
            }
            return ret;
        }

        public bool ExistsPrevTask()
        {
            return prevTask != null;
        }
    }
}
