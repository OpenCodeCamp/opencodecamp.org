namespace OpenCodeCamp.Services.Marketing.Domain.Events
{
    using MediatR;

    class NewsletterSubscriptionStatusChangedToCancelledDomainEvent
    : INotification
    {
        public int NewsletterSubscriptionId { get; }

        public NewsletterSubscriptionStatusChangedToCancelledDomainEvent(int newsletterSubscriptionId)
        {
            this.NewsletterSubscriptionId = newsletterSubscriptionId;
        }
    }
}