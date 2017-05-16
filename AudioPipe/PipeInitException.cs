using System;

namespace AudioPipe
{
    [Serializable]
    public class PipeInitException : Exception
    {
        public PipeInitException()
        {
        }

        public PipeInitException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
