using AudioPipe.Services;
using NAudio.CoreAudioApi;
using System;
using System.Runtime.Serialization;

namespace AudioPipe.Audio
{
    /// <summary>
    /// Represents an error when creating a <see cref="Pipe"/>.
    /// </summary>
    [Serializable]
    public class PipeInitException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipeInitException"/> class.
        /// </summary>
        public PipeInitException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PipeInitException" /> class with a specified error message.</summary>
        /// <param name="message">The message that describes the error. </param>
        public PipeInitException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PipeInitException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified. </param>
        public PipeInitException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeInitException" /> class with a message determined
        /// by an HRESULT.
        /// </summary>
        /// <param name="hresult">HRESULT of the failed operation.</param>
        /// <param name="device">Audio device relevant to the operation.</param>
        public PipeInitException(int hresult, MMDevice device)
            : this(HResultService.GetAudioDeviceError(hresult, device))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PipeInitException" /> class with serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination. </param>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="SerializationException">The class name is <see langword="null" /> or <see cref="Exception.HResult" /> is zero (0). </exception>
        protected PipeInitException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
