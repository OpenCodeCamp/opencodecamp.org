using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF;
using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
using OpenCodeCamp.Services.Marketing.Infrastructure;

namespace Marketing.FunctionalTests
{
    public class CustomWebApplicationFactory<TStartup>
       : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's ApplicationDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<MarketingContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<MarketingContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                services.AddDbContext<IntegrationEventLogContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<MarketingContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                        Utilities.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }

    public static class Utilities
    {
        public static void InitializeDbForTests(MarketingContext db)
        {
            db.NewsletterSubscriptionTokenTypes.AddRange(GetPredefinedNewsletterSubscriptionTokenTypes());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(MarketingContext db)
        {
            db.NewsletterSubscriptionStatus.RemoveRange(GetPredefinedNewsletterSubscriptionStatus());
            InitializeDbForTests(db);
        }

        private static IEnumerable<NewsletterSubscriptionStatus> GetPredefinedNewsletterSubscriptionStatus()
        {
            return new List<NewsletterSubscriptionStatus>()
            {
                NewsletterSubscriptionStatus.Submitted,
                NewsletterSubscriptionStatus.Confirmed,
                NewsletterSubscriptionStatus.AwaitingCancellationValidation,
                NewsletterSubscriptionStatus.Cancelled
            };
        }

        private static IEnumerable<NewsletterSubscriptionTokenType> GetPredefinedNewsletterSubscriptionTokenTypes()
        {
            return new List<NewsletterSubscriptionTokenType>()
            {
                NewsletterSubscriptionTokenType.Confirmation,
                NewsletterSubscriptionTokenType.Cancellation
            };
        }
    }
}