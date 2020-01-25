namespace OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.Events
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;
    using System;

    /// <summary>
    /// Event sent by us when a mail is sended after a new subscription to the newsletter.
    /// This event is a response when we handle a NewsletterSubscriptionSubmittedIntegrationEvent.
    /// </summary>
    public class NewsletterSubscriptionSubmittedEmailSuccededIntegrationEvent : IntegrationEvent
    {
        public string EmailAddress { get; }
        public DateTime EmailSendingDate { get; }

        public NewsletterSubscriptionSubmittedEmailSuccededIntegrationEvent(string email, DateTime mailSent)
        {
            this.EmailAddress = email;
            this.EmailSendingDate = mailSent;
        }
    }
}