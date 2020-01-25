namespace OpenCodeCamp.Services.Marketing.Api.Application.Commands
{
    using MediatR;
    using Microsoft.Extensions.Logging;
    using OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents;
    using OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents.Events;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using OpenCodeCamp.Services.Marketing.Infrastructure.Idempotency;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    // Regular CommandHandler
    public class SubscribeToNewsletterCommandHandler
            : IRequestHandler<SubscribeToNewsletterCommand, bool>
    {
        private readonly INewsletterSubscriptionRepository _newsletterSubscriptionRepository;
        private readonly IMediator _mediator;
        private readonly IMarketingIntegrationEventService _marketingIntegrationEventService;
        private readonly ILogger<SubscribeToNewsletterCommandHandler> _logger;

        // Using DI to inject infrastructure persistence Repositories
        public SubscribeToNewsletterCommandHandler(IMediator mediator,
            IMarketingIntegrationEventService marketingIntegrationEventService,
            INewsletterSubscriptionRepository newsletterSubscriptionRepository,
            //IIdentityService identityService,
            ILogger<SubscribeToNewsletterCommandHandler> logger)
        {
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository ?? throw new ArgumentNullException(nameof(newsletterSubscriptionRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _marketingIntegrationEventService = marketingIntegrationEventService ?? throw new ArgumentNullException(nameof(marketingIntegrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(SubscribeToNewsletterCommand message, CancellationToken cancellationToken)
        {
            if (await this._newsletterSubscriptionRepository.IsEmailAlreadyUsesAsync(message.EmailAddress))
            {
                _logger.LogInformation("Email already use in newsletter subscription");
                return false;
            }

            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate
            var newsletterSubscription = new NewsletterSubscription(message.Language, message.EmailAddress);

            _logger.LogInformation("----- Creating Newsletter Subscription - NewsletterSubscription: {@NewsletterSubscription}", newsletterSubscription);

            this._newsletterSubscriptionRepository.Add(newsletterSubscription);

            return await _newsletterSubscriptionRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);
        }
    }

    // Use for Idempotency in Command process
    public class SubscribeToNewsletterIdentifiedCommandHandler : IdentifiedCommandHandler<SubscribeToNewsletterCommand, bool>
    {
        public SubscribeToNewsletterIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<SubscribeToNewsletterCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;// Ignore duplicate requests for creating newsletter subscription.
        }
    }
}