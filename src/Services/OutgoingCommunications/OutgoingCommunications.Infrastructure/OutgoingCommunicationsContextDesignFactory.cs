namespace OpenCodeCamp.Services.OutgoingCommunications.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.EntityFrameworkCore.Storage;
    using MediatR;
    using System.Data;
    using Microsoft.EntityFrameworkCore;
    using OpenCodeCamp.Services.OutgoingCommunications.Domain.Seedwork;
    using OpenCodeCamp.Services.OutgoingCommunications.Infrastructure.EntityConfigurations;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using System.IO;

    public class OutgoingCommunicationsContextDesignFactory : IDesignTimeDbContextFactory<OutgoingCommunicationsContext>
    {
        public OutgoingCommunicationsContext CreateDbContext(string[] args)
        {
            // Build config
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<OutgoingCommunicationsContext>()
                .UseSqlServer(config.GetConnectionString("ConnectionString"));

            return new OutgoingCommunicationsContext(optionsBuilder.Options, new NoMediator());
        }

        class NoMediator : IMediator
        {
            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.FromResult<TResponse>(default(TResponse));
            }

            public Task Send(IRequest request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.CompletedTask;
            }
        }
    }
}