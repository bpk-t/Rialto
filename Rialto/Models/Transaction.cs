using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public class Transaction : IWorkerTask
    {
        public List<IWorkerTask> Tasks { get; set; }

        public Transaction()
        {
            Tasks = new List<IWorkerTask>();
        }

        public Transaction(List<IWorkerTask> tasks)
        {
            this.Tasks = tasks;
        }

        public void Execute()
        {
            Tasks.ForEach(x => x.Execute());
        }

        public void Rollback()
        {
            Tasks.ForEach(x => x.Rollback());
        }

        public void AddWorkerTask(IWorkerTask task)
        {
            Tasks.Add(task);
        }
    }
}
