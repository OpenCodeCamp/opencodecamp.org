namespace OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.Events
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;

    public class NewsletterSubscriptionConfirmedEmailFailedIntegrationEvent : IntegrationEvent
    {
        public string EmailAddress { get; }

        public NewsletterSubscriptionConfirmedEmailFailedIntegrationEvent(string email)
        {
            this.EmailAddress = email;
        }
    }
}