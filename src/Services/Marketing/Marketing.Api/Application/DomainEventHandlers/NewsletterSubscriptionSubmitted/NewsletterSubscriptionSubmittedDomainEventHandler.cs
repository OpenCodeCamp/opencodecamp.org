namespace OpenCodeCamp.Services.Marketing.Api.Application.DomainEventHandlers.NewsletterSubscriptionSubmitted
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
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure.Configuration;
    using Microsoft.Extensions.Options;

    public class NewsletterSubscriptionSubmittedDomainEventHandler
                   : INotificationHandler<NewsletterSubscriptionSubmittedDomainEvent>
    {
        private readonly INewsletterSubscriptionRepository _newsletterSubscriptionRepository;
        private readonly IMarketingIntegrationEventService _marketingIntegrationEventService;
        private readonly ILoggerFactory _logger;
        private readonly MarketingSettings settings;

        public NewsletterSubscriptionSubmittedDomainEventHandler(
            ILoggerFactory logger,
            INewsletterSubscriptionRepository newsletterSubscriptionRepository,
            IMarketingIntegrationEventService marketingIntegrationEventService,
            IOptionsSnapshot<MarketingSettings> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(settings.Value.WebFrontOfficeUrl))
            {
                throw new ArgumentNullException(nameof(settings.Value.WebFrontOfficeUrl));
            }

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._newsletterSubscriptionRepository = newsletterSubscriptionRepository ?? throw new ArgumentNullException(nameof(newsletterSubscriptionRepository));
            this._marketingIntegrationEventService = marketingIntegrationEventService;
            this.settings = settings.Value;
        }

        public async Task Handle(NewsletterSubscriptionSubmittedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<NewsletterSubscriptionSubmittedDomainEvent>()
                .LogTrace("Newsletter ubscription with Id: {NewsletterSubscriptionId} has been successfully submitted.",
                    domainEvent.NewsletterSubscription.Id);

            var newsletterSubscription = await _newsletterSubscriptionRepository.GetAsync(domainEvent.NewsletterSubscription.Id);
            string confirmationToken = newsletterSubscription.GetConfirmationToken();

            // TODO: find a smarter way to build this URL
            string confirmationUrl = this.settings.WebFrontOfficeUrl + "confirm-newsletter-subscription/" + newsletterSubscription.EmailAddress + "/" + confirmationToken;

            var integrationEvent = new NewsletterSubscriptionSubmittedIntegrationEvent(
                newsletterSubscription.EmailAddress, confirmationUrl, newsletterSubscription.Language);

            await _marketingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
        }
    }
}