namespace OpenCodeCamp.Services.Marketing.Api
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF;
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure;
    using OpenCodeCamp.Services.Marketing.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Serilog;
    using System;
    using System.IO;
    using Microsoft.Extensions.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Autofac.Extensions.DependencyInjection;

    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);

                IHost host = CreateHostBuilder2(args).Build();

                Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var marketingContextLogger = services.GetRequiredService<ILogger<MarketingContext>>();
                    var marketingContext = services.GetService<MarketingContext>();

                    Log.Information("Applying migrations ({ApplicationContext}) to {contextName}", AppName, nameof(MarketingContext));
                    marketingContext.Database.EnsureCreated();
                    marketingContext.Database.Migrate();
                    new MarketingContextSeed().SeedAsync(marketingContext).Wait();
                    Log.Information("Migrations ({ApplicationContext}) to {contextName} successfull applied", AppName, nameof(MarketingContext));

                    var integrationEventLogContextLogger = services.GetRequiredService<ILogger<IntegrationEventLogContext>>();
                    var integrationEventLogContext = services.GetService<IntegrationEventLogContext>();

                    Log.Information("Applying migrations ({ApplicationContext}) to {contextName}", AppName, nameof(IntegrationEventLogContext));
                    integrationEventLogContext.Database.EnsureCreated();
                    //string creationSqlScript = integrationEventLogContext.Database.GenerateCreateScript();
                    //integrationEventLogContext.Database.Migrate();
                    Log.Information("Migrations ({ApplicationContext}) to {contextName} successfull applied", AppName, nameof(IntegrationEventLogContext));
                }


                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseKestrel();
                    webBuilder.ConfigureServices(services => services.AddAutofac());
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseConfiguration(GetConfiguration());
                    webBuilder.UseSerilog();
                    webBuilder.UseIISIntegration();
                });

        public static IHostBuilder CreateHostBuilder2(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices(services => services.AddAutofac())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseKestrel();
                    webBuilder.ConfigureServices(services => services.AddAutofac());
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseConfiguration(GetConfiguration());
                    webBuilder.UseSerilog();
                });

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            return builder.Build();
        }
    }
}