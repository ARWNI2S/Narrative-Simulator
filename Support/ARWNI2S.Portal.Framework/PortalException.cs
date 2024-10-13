using ARWNI2S.Infrastructure;

namespace ARWNI2S.Portal.Framework
{
    public class PortalException : NI2SException
    {
        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        public PortalException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PortalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message.
        /// </summary>
        /// <param name="messageFormat">The exception message format.</param>
        /// <param name="args">The exception message arguments.</param>
        public PortalException(string messageFormat, params object[] args)
            : base(messageFormat, args)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public PortalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
