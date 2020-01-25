namespace OpenCodeCamp.Services.Marketing.Domain.Events
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MediatR;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;

    class NewsletterSubscriptionStatusChangedToConfirmedDomainEvent
     : INotification
    {
        public int NewsletterSubscriptionId { get; }

        public NewsletterSubscriptionStatusChangedToConfirmedDomainEvent(int newsletterSubscriptionId)
        {
            this.NewsletterSubscriptionId = newsletterSubscriptionId;
        }
    }
}