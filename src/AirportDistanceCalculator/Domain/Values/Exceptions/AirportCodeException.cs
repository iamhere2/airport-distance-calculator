using System;
using System.Runtime.Serialization;

namespace AirportDistanceCalculator.Domain.Values.Exceptions
{
    /// <summary>Exception for wrong airport codes</summary>
    public class AirportCodeException : ApplicationException
    {
        /// <summary>Constructor</summary>
        public AirportCodeException(string code) : base(BuildMessage(code))
        {
        }

        private static string BuildMessage(string? code)
            => $"Wrong or unknown airport code: \"{code ?? "(?)"}\"."
               + $" Airport code can have only three upper-case latin letters";

        /// <summary>Constructor</summary>
        public AirportCodeException(string code, Exception innerException) : base(BuildMessage(code), innerException)
        {
        }

        /// <summary>Constructor</summary>
        protected AirportCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>Constructor</summary>
        public AirportCodeException() : base(BuildMessage(null))
        {
        }
    }
}
