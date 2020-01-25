namespace OpenCodeCamp.Services.Marketing.Domain.Events
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MediatR;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;

    /// <summary>
    /// Event used when a new newsletter subscription is submitted
    /// </summary>
    public class NewsletterSubscriptionSubmittedDomainEvent : INotification
    {
        public string EmailAddress { get; }
        public string ConfirmationToken { get; }
        public NewsletterSubscription NewsletterSubscription { get; }
        public string Language { get; }

        public NewsletterSubscriptionSubmittedDomainEvent(NewsletterSubscription newsletterSubscription,
           string language, string emailAddress, string confirmationToken)
        {
            this.NewsletterSubscription = newsletterSubscription;
            this.EmailAddress = emailAddress;
            this.ConfirmationToken = confirmationToken;
            this.Language = language;
        }
    }
}