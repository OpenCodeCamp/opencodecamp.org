namespace OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;
    using System;
    using System.Threading.Tasks;

    public interface IMarketingIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}