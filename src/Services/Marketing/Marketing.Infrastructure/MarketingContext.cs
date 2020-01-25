namespace OpenCodeCamp.Services.Marketing.Infrastructure
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Storage;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using OpenCodeCamp.Services.Marketing.Domain.Seedwork;
    using OpenCodeCamp.Services.Marketing.Infrastructure.EntityConfigurations;
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    public class MarketingContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "marketing";
        public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
        public DbSet<NewsletterSubscriptionStatus> NewsletterSubscriptionStatus { get; set; }
        public DbSet<NewsletterSubscriptionToken> NewsletterSubscriptionTokens { get; set; }
        public DbSet<NewsletterSubscriptionTokenType> NewsletterSubscriptionTokenTypes { get; set; }

        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        private MarketingContext(DbContextOptions<MarketingContext> options) : base(options) { }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public MarketingContext(DbContextOptions<MarketingContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            System.Diagnostics.Debug.WriteLine("MarketingContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new NewsletterSubscriptionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new NewsletterSubscriptionStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new NewsletterSubscriptionTokenEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new NewsletterSubscriptionTokenTypeEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}