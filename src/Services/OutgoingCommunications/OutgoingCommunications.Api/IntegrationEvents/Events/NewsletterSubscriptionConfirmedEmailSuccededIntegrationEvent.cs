namespace OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.Events
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;
    using System;

    public class NewsletterSubscriptionConfirmedEmailSuccededIntegrationEvent : IntegrationEvent
    {
        public string EmailAddress { get; }
        public DateTime EmailSendingDate { get; }

        public NewsletterSubscriptionConfirmedEmailSuccededIntegrationEvent(string email, DateTime mailSent)
        {
            this.EmailAddress = email;
            this.EmailSendingDate = mailSent;
        }
    }
}