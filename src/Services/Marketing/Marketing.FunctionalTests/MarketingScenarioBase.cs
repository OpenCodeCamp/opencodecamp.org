namespace Marketing.FunctionalTests
{
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF;
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure;
    using OpenCodeCamp.Services.Marketing.Infrastructure;
    using System;
    using System.IO;
    using System.Reflection;

    public class MarketingScenarioBase
    {
        public TestServer CreateServer()
        {
            try
            {
                string path = Assembly.GetAssembly(typeof(MarketingScenarioBase))
                    .Location;

                IHostBuilder hostBuilder = CreateHostBuilder2(path);
                IHost host = hostBuilder.Build();
                Microsoft.AspNetCore.TestHost.TestServer testServer = host.GetTestServer();
                //TestServer testServer = (TestServer)host.Services.GetRequiredService<IServer>();

                testServer.Host
                    .MigrateDbContext<MarketingContext>((context, services) =>
                    {
                        //var env = services.GetService<IWebHostEnvironment>();
                        //var settings = services.GetService<IOptions<MarketingSettings>>();
                        //var logger = services.GetService<ILogger<MarketingContextSeed>>();

                        new MarketingContextSeed()
                        .SeedAsync(context/*, env, settings, logger*/)
                        .Wait();
                    })
                    .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

                return testServer;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder2(string path) =>
            Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices(services => services.AddAutofac())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services => services.AddAutofac());
                    webBuilder.UseStartup<MarketingTestsStartup>();
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                })
                .UseContentRoot(Path.GetDirectoryName(path))
                    .ConfigureAppConfiguration(cb =>
                    {
                        cb.AddJsonFile("appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                    });

        public static class PostUrls
        {
            public static string NewsletterSubscriptions = "api/v1/newslettersubscriptions";

            public static string SubscribeToNewsletter = NewsletterSubscriptions + "/create";
        }
    }

    public static class hshshs
    {


        public static IHostBuilder CreateHostBuilder2(string path) =>
            Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices(services => services.AddAutofac())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services => services.AddAutofac());
                    webBuilder.UseStartup<MarketingTestsStartup>();
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                })
                .UseContentRoot(Path.GetDirectoryName(path))
                    .ConfigureAppConfiguration(cb =>
                    {
                        cb.AddJsonFile("appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                    });
    }
}