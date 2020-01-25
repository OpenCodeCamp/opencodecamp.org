namespace OpenCodeCamp.Services.Marketing.Domain.Events
{
    using MediatR;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;

    /// <summary>
    /// Event used when a new newsletter subscription is confirmed
    /// </summary>
    public class NewsletterSubscriptionConfirmedDomainEvent : INotification
    {
        public string EmailAddress { get; }
        public string Language { get; }
        public NewsletterSubscription NewsletterSubscription { get; }

        public NewsletterSubscriptionConfirmedDomainEvent(NewsletterSubscription newsletterSubscription, string language, string emailAddress)
        {
            this.Language = language;
            this.EmailAddress = emailAddress;
            this.NewsletterSubscription = newsletterSubscription;
        }
    }
}