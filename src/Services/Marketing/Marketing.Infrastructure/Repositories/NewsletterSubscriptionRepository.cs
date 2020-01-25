namespace OpenCodeCamp.Services.Marketing.Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using OpenCodeCamp.Services.Marketing.Domain.Seedwork;
    using OpenCodeCamp.Services.Marketing.Domain.Exceptions;
    using System;
    using System.Threading.Tasks;

    public class NewsletterSubscriptionRepository
        : INewsletterSubscriptionRepository
    {
        private readonly MarketingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public NewsletterSubscriptionRepository(MarketingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public NewsletterSubscription Add(NewsletterSubscription newsletterSubscription)
        {
            return _context.NewsletterSubscriptions.Add(newsletterSubscription).Entity;

        }

        public async Task<NewsletterSubscription> GetAsync(int newsletterSubscriptionId)
        {
            var newsletterSubscription = await _context.NewsletterSubscriptions.FindAsync(newsletterSubscriptionId);
            if (newsletterSubscription != null)
            {
                await _context.Entry(newsletterSubscription)
                    .Reference(i => i.Status)
                    .LoadAsync();
            }

            return newsletterSubscription;
        }

        public void Update(NewsletterSubscription newsletterSubscription)
        {
            _context.Entry(newsletterSubscription).State = EntityState.Modified;
        }

        public async Task<bool> IsEmailAlreadyUsesAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            string lowercaseEmail = email.Trim().ToLower();
            if (!await this._context.NewsletterSubscriptions.AnyAsync())
            {
                return false;
            }
            return await this._context.NewsletterSubscriptions.SingleOrDefaultAsync(x => x.EmailAddress == lowercaseEmail) != null;
        }

        public async Task<NewsletterSubscription> GetAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (!await this._context.NewsletterSubscriptions.AnyAsync())
            {
                return null;
            }

            string lowercaseEmail = email.Trim().ToLower();
            var newsletterSubscription = await this._context.NewsletterSubscriptions.SingleOrDefaultAsync(x => x.EmailAddress == lowercaseEmail);
            if (newsletterSubscription != null)
            {
                await _context.Entry(newsletterSubscription)
                    .Reference(i => i.Status)
                    .LoadAsync();
            }

            return newsletterSubscription;
        }
    }
}