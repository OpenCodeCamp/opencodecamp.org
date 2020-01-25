namespace OpenCodeCamp.BuildingBlocks.EventBus.Abstractions
{
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;
    using System.Threading.Tasks;

    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
       where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}