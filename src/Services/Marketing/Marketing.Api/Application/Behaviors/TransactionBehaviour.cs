namespace OpenCodeCamp.Services.Marketing.Api.Application.Behaviors
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using OpenCodeCamp.BuildingBlocks.EventBus.Extensions;
    using OpenCodeCamp.Services.Marketing.Infrastructure;
    using Microsoft.Extensions.Logging;
    using OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents;
    using Serilog.Context;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly MarketingContext _dbContext;
        private readonly IMarketingIntegrationEventService _marketingIntegrationEventService;

        public TransactionBehaviour(MarketingContext dbContext,
            IMarketingIntegrationEventService marketingIntegrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            this._dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            this._marketingIntegrationEventService = marketingIntegrationEventService ?? throw new ArgumentException(nameof(marketingIntegrationEventService));
            this._logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (var transaction = await _dbContext.BeginTransactionAsync())
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        this._logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        this._logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        await this._dbContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                    await this._marketingIntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}