using System;
using System.Runtime.Serialization;

namespace Business.Validation
{
    [Serializable]
    public class MarketException : Exception
    {
        public MarketException() : base()
        {
        }

        public MarketException(string message) : base(message)
        {
        }

        public MarketException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MarketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
