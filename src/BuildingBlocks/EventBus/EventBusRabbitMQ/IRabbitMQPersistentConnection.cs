namespace OpenCodeCamp.BuildingBlocks.EventBusRabbitMQ
{
    using RabbitMQ.Client;
    using System;

    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}