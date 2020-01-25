namespace OpenCodeCamp.Services.OutgoingCommunications.Domain.Exceptions
{
    using System;

    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class OutgoingCommunicationsDomainException : Exception
    {
        public OutgoingCommunicationsDomainException()
        { }

        public OutgoingCommunicationsDomainException(string message)
            : base(message)
        { }

        public OutgoingCommunicationsDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}