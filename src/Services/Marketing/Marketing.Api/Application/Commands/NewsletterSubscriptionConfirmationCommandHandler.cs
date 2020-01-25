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
    public class NewsletterSubscriptionConfirmationCommandHandler
     : IRequestHandler<NewsletterSubscriptionConfirmationCommand, bool>
    {
        private readonly INewsletterSubscriptionRepository _newsletterSubscriptionRepository;
        //private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly IMarketingIntegrationEventService _marketingIntegrationEventService;
        private readonly ILogger<NewsletterSubscriptionConfirmationCommandHandler> _logger;

        // Using DI to inject infrastructure persistence Repositories
        public NewsletterSubscriptionConfirmationCommandHandler(IMediator mediator,
            IMarketingIntegrationEventService marketingIntegrationEventService,
            INewsletterSubscriptionRepository newsletterSubscriptionRepository,
            //IIdentityService identityService,
            ILogger<NewsletterSubscriptionConfirmationCommandHandler> logger)
        {
            _newsletterSubscriptionRepository = newsletterSubscriptionRepository ?? throw new ArgumentNullException(nameof(newsletterSubscriptionRepository));
            //_identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _marketingIntegrationEventService = marketingIntegrationEventService ?? throw new ArgumentNullException(nameof(marketingIntegrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(NewsletterSubscriptionConfirmationCommand message, CancellationToken cancellationToken)
        {
            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate
            NewsletterSubscription newsletterSubscription = await this._newsletterSubscriptionRepository.GetAsync(message.EmailAddress);

            _logger.LogInformation("----- Confirm Newsletter Subscription - NewsletterSubscription: {@NewsletterSubscription}", newsletterSubscription);

            newsletterSubscription.Confirm(message.ConfirmationToken);

            _logger.LogInformation("----- Newsletter Subscription Confirmed");

            // Commented by atorris, 28/08/2019: The integration event will be sent by the domain.
            //// Add Integration event to send the mail to confirm the confirmation to the user.
            //var integrationEvent = new NewsletterSubscriptionConfirmedIntegrationEvent(message.EmailAddress);
            //await _marketingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);

            return await _newsletterSubscriptionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }

    // Use for Idempotency in Command process
    public class NewsletterSubscriptionConfirmationIdentifiedCommandHandler : IdentifiedCommandHandler<NewsletterSubscriptionConfirmationCommand, bool>
    {
        public NewsletterSubscriptionConfirmationIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<NewsletterSubscriptionConfirmationCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;// Ignore duplicate requests for creating newsletter subscription.
        }
    }
}