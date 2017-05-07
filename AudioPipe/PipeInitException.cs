using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPipe
{
    public class PipeInitException : Exception
    {
        public PipeInitException()
            : base()
        {
        }

        public PipeInitException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
