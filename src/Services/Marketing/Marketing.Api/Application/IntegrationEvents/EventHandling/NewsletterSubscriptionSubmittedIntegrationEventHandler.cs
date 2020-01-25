//namespace OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents.EventHandling
//{
//    using OpenCodeCamp.BuildingBlocks.EventBus.Abstractions;
//    using OpenCodeCamp.BuildingBlocks.EventBus.Extensions;
//    using System.Threading.Tasks;
//    using Events;
//    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
//    using MediatR;
//    using System;
//    using OpenCodeCamp.Services.Marketing.Api.Application.Commands;
//    using Microsoft.Extensions.Logging;
//    using Serilog.Context;
//    using OpenCodeCamp.Services.Marketing.Api.Application.Behaviors;
//    using OpenCodeCamp.Services.Marketing.Api;

//    public class NewsletterSubscriptionSubmittedIntegrationEventHandler :
//        IIntegrationEventHandler<NewsletterSubscriptionSubmittedIntegrationEvent>
//    {
//        private readonly IMediator _mediator;
//        private readonly ILogger<NewsletterSubscriptionSubmittedIntegrationEventHandler> _logger;

//        public NewsletterSubscriptionSubmittedIntegrationEventHandler(
//            IMediator mediator,
//            ILogger<NewsletterSubscriptionSubmittedIntegrationEventHandler> logger)
//        {
//            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task Handle(NewsletterSubscriptionSubmittedIntegrationEvent @event)
//        {
//            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
//            {
//                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

//                var command = new SendNewsletterSubscriptionSubmittedEmailCommand(@event.EmailAddress, @event.ConfirmationToken);

//                _logger.LogInformation(
//                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
//                    command.GetGenericTypeName(),
//                    nameof(command.EmailAddress),
//                    command.EmailAddress,
//                    command);

//                await _mediator.Send(command);
//            }
//        }
//    }
//}