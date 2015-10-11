using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public interface IWorkerTask
    {
        void Execute();

        void Rollback();
    }
}
