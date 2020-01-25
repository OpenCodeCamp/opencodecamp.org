namespace OpenCodeCamp.Services.Marketing.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure.Configuration;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using OpenCodeCamp.Services.Marketing.Infrastructure;

    public class MarketingContextSeed
    {
        public async Task SeedAsync(MarketingContext context)//, IOptions<MarketingSettings> settings, ILogger<MarketingContextSeed> logger
        {
            using (context)
            {
                context.Database.Migrate();

                if (!context.NewsletterSubscriptionStatus.Any())
                {
                    context.NewsletterSubscriptionStatus.AddRange(GetPredefinedNewsletterSubscriptionStatus());
                }

                if (!context.NewsletterSubscriptionTokenTypes.Any())
                {
                    context.NewsletterSubscriptionTokenTypes.AddRange(GetPredefinedNewsletterSubscriptionTokenTypes());
                }

                await context.SaveChangesAsync();
            }
        }

        private IEnumerable<NewsletterSubscriptionStatus> GetPredefinedNewsletterSubscriptionStatus()
        {
            return new List<NewsletterSubscriptionStatus>()
            {
                NewsletterSubscriptionStatus.Submitted,
                NewsletterSubscriptionStatus.Confirmed,
                NewsletterSubscriptionStatus.AwaitingCancellationValidation,
                NewsletterSubscriptionStatus.Cancelled
            };
        }

        private IEnumerable<NewsletterSubscriptionTokenType> GetPredefinedNewsletterSubscriptionTokenTypes()
        {
            return new List<NewsletterSubscriptionTokenType>()
            {
                NewsletterSubscriptionTokenType.Confirmation,
                NewsletterSubscriptionTokenType.Cancellation
            };
        }
    }
}