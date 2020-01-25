namespace OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.Events
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;

    /// <summary>
    /// Event sent by us when we failed to send an email after a new subscription to the newsletter.
    /// This event is a response when we handle a NewsletterSubscriptionSubmittedIntegrationEvent.
    /// </summary>
    public class NewsletterSubscriptionSubmittedEmailFailedIntegrationEvent : IntegrationEvent
    {
        public string EmailAddress { get; }

        public NewsletterSubscriptionSubmittedEmailFailedIntegrationEvent(string email)
        {
            this.EmailAddress = email;
        }
    }
}