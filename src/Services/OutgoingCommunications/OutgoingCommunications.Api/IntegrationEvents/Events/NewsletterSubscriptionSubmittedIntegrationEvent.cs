namespace OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.Events
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;

    /// <summary>
    /// Event sent when a new newsletter subscription is submitted
    /// Sent by the Marketing
    /// </summary>
    public class NewsletterSubscriptionSubmittedIntegrationEvent : IntegrationEvent
    {
        public string EmailAddress { get; set; }
        //public string ConfirmationToken { get; set; }
        public string ConfirmationUrl { get; set; }
        public string Language { get; set; }

        public NewsletterSubscriptionSubmittedIntegrationEvent(string language, string emailAddress, string confirmationUrl)
        {
            this.Language = language;
            this.EmailAddress = emailAddress;
            this.ConfirmationUrl = confirmationUrl;
        }
    }
}