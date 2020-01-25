namespace OpenCodeCamp.Services.Marketing.Domain.Events
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MediatR;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;

    class NewsletterSubscriptionStatusChangedToAwaitingCancellationValidationDomainEvent
    : INotification
    {
        public int NewsletterSubscriptionId { get; }
        public string EmailAddress { get; }
        public string CancellationToken { get; }

        public NewsletterSubscriptionStatusChangedToAwaitingCancellationValidationDomainEvent(int newsletterSubscriptionId,
            string emailAddress, string cancellationToken)
        {
            this.NewsletterSubscriptionId = newsletterSubscriptionId;
            this.EmailAddress = emailAddress;
            this.CancellationToken = cancellationToken;
        }
    }
}