namespace OpenCodeCamp.Services.Marketing.Api.Application.DomainEventHandlers.NewsletterSubscriptionConfirmed
{
    using MediatR;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using Microsoft.Extensions.Logging;
    using Marketing.Api.Application.IntegrationEvents;
    using Marketing.Api.Application.IntegrationEvents.Events;
    using Marketing.Domain.Events;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class NewsletterSubscriptionConfirmedDomainEventHandler
                   : INotificationHandler<NewsletterSubscriptionConfirmedDomainEvent>
    {
        private readonly INewsletterSubscriptionRepository _newsletterSubscriptionRepository;
        private readonly IMarketingIntegrationEventService _marketingIntegrationEventService;
        private readonly ILoggerFactory _logger;

        public NewsletterSubscriptionConfirmedDomainEventHandler(
            ILoggerFactory logger,
            INewsletterSubscriptionRepository newsletterSubscriptionRepository,
            IMarketingIntegrationEventService marketingIntegrationEventService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository ?? throw new ArgumentNullException(nameof(newsletterSubscriptionRepository));
            _marketingIntegrationEventService = marketingIntegrationEventService;
        }

        public async Task Handle(NewsletterSubscriptionConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<NewsletterSubscriptionConfirmedDomainEvent>()
                .LogTrace("Newsletter subscription with Id: {NewsletterSubscriptionId} has been successfully confirmed.",
                    domainEvent.NewsletterSubscription.Id);


            var integrationEvent = new NewsletterSubscriptionConfirmedIntegrationEvent(
                domainEvent.EmailAddress);

            await _marketingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
        }
    }
}