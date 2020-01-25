namespace OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.Events
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;

    public class NewsletterSubscriptionConfirmedIntegrationEvent : IntegrationEvent
    {
        public string EmailAddress { get; set; }
        public string Language { get; set; }

        public NewsletterSubscriptionConfirmedIntegrationEvent(string language, string emailAddress)
        {
            this.Language = language;
            this.EmailAddress = emailAddress;
        }
    }
}