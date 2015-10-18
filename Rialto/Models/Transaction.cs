using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public class Transaction<T> : IWorkerTask where T : IWorkerTask
    {
        public List<T> Tasks { get; set; }

        public Transaction(IEnumerable<T> tasks)
        {
            this.Tasks = tasks.ToList();
        }

        public void Execute()
        {
            Tasks.ForEach(x => x.Execute());
        }

        public void Rollback()
        {
            Tasks.ForEach(x => x.Rollback());
        }

        public void AddWorkerTask(T task)
        {
            Tasks.Add(task);
        }
    }
}
