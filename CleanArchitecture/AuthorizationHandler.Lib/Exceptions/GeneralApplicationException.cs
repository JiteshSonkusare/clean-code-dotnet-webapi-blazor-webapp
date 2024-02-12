using System.Runtime.Serialization;

namespace AuthorizationHandler.Lib.Exceptions
{
    [Serializable]
    public class GeneralApplicationException : ApplicationException
    {
        /// <summary>
        /// General argumentless constructor.
        /// </summary>
        public GeneralApplicationException() : base() { }

        /// <summary>
        /// Constructor which allows user to pass exception message.
        /// </summary>
        /// <param name="message"></param>
        public GeneralApplicationException(string message) : base(message) { }

        /// <summary>
        /// Exception serialization constructor.
        /// </summary>
        /// <param name="serialiizationInfo"></param>
        /// <param name="context"></param>
        public GeneralApplicationException(SerializationInfo serialiizationInfo, StreamingContext context) : base(serialiizationInfo, context) { }

        /// <summary>
        /// Exception which allows to attach to itself another, inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public GeneralApplicationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
