namespace OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using OpenCodeCamp.BuildingBlocks.EventBus.Abstractions;
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;
    using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF;
    using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF.Services;
    using OpenCodeCamp.Services.Marketing.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;

    public class MarketingIntegrationEventService: IMarketingIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly MarketingContext _marketingContext;
        private readonly IntegrationEventLogContext _eventLogContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<MarketingIntegrationEventService> _logger;

        public MarketingIntegrationEventService(IEventBus eventBus,
            MarketingContext marketingContext,
            IntegrationEventLogContext eventLogContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<MarketingIntegrationEventService> logger)
        {
            this._marketingContext = marketingContext ?? throw new ArgumentNullException(nameof(marketingContext));
            this._eventLogContext = eventLogContext ?? throw new ArgumentNullException(nameof(eventLogContext));
            this._integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            this._eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this._eventLogService = integrationEventLogServiceFactory(marketingContext.Database.GetDbConnection());
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", logEvt.EventId, Program.AppName, logEvt.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    _eventBus.Publish(logEvt.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", logEvt.EventId, Program.AppName);

                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            await this._eventLogService.SaveEventAsync(evt, this._marketingContext.GetCurrentTransaction());
        }
    }
}