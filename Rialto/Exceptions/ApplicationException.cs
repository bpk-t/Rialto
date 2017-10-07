using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Exceptions
{
    public class ApplicationException : Exception
    {
        public ApplicationException()
        {

        }

        public ApplicationException(string message): base(message)
        {

        }

        public ApplicationException(string message, System.Exception inner): base(message, inner)
        {

        }
    }
}
