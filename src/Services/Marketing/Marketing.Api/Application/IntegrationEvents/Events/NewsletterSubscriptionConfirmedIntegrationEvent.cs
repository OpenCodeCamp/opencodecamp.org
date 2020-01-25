namespace OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents.Events
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;

    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class NewsletterSubscriptionConfirmedIntegrationEvent : IntegrationEvent
    {
        public string EmailAddress { get; set; }

        public NewsletterSubscriptionConfirmedIntegrationEvent(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }
    }
}