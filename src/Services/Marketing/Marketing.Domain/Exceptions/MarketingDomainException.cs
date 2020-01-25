using System;

namespace OpenCodeCamp.Services.Marketing.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class MarketingDomainException : Exception
    {
        public MarketingDomainException()
        { }

        public MarketingDomainException(string message)
            : base(message)
        { }

        public MarketingDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}