namespace OpenCodeCamp.Services.Marketing.Infrastructure
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Configuration;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using OpenCodeCamp.Services.Marketing.Domain.Seedwork;
    using OpenCodeCamp.Services.Marketing.Infrastructure.EntityConfigurations;
    using System;
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class MarketingContextDesignFactory : IDesignTimeDbContextFactory<MarketingContext>
    {
        public MarketingContext CreateDbContext(string[] args)
        {
            // Build config
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MarketingContext>()
                .UseSqlServer(config.GetConnectionString("ConnectionString"));

            return new MarketingContext(optionsBuilder.Options, new NoMediator());
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