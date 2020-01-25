namespace OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents.Events
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;

    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class NewsletterSubscriptionSubmittedIntegrationEvent : IntegrationEvent
    {
        public string EmailAddress { get; set; }
        public string ConfirmationUrl { get; set; }
        public string Language { get; set; }

        public NewsletterSubscriptionSubmittedIntegrationEvent(string emailAddress, string confirmationUrl, string language)
        {
            this.EmailAddress = emailAddress;
            this.ConfirmationUrl = confirmationUrl;
            this.Language = language;
        }
    }
}