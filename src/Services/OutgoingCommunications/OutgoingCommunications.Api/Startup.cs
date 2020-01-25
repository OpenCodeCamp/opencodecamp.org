namespace OpenCodeCamp.Services.OutgoingCommunications.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Autofac;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using OpenCodeCamp.BuildingBlocks.EventBus;
    using OpenCodeCamp.BuildingBlocks.EventBus.Abstractions;
    using OpenCodeCamp.BuildingBlocks.EventBusRabbitMQ;
    using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF;
    using OpenCodeCamp.Services.OutgoingCommunications.Api.Infrastructure.AutofacModules;
    using OpenCodeCamp.Services.OutgoingCommunications.Api.Infrastructure.Configuration;
    using OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.EventHandling;
    using OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.Events;
    using RabbitMQ.Client;

    //https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Payment/Payment.API
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //public OpenCodeCamp.OutgoingCommunications.EmailDocumentsLibrary.Services.IRazorViewToStringRenderer RazorViewToStringRenderer { get; internal set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OutgoingCommunicationsSettings>(Configuration);

            services.AddCustomDbContext(Configuration);

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrWhiteSpace(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }

                if (!string.IsNullOrWhiteSpace(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrWhiteSpace(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            RegisterEventBus(services);
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you. If you
        // need a reference to the container, you need to use the
        // "Without ConfigureContainer" mechanism shown later.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var mailSettings = new MailSettings();
            Configuration.GetSection(OutgoingCommunicationsSettingsKeys.MailSettingsSectionName).Bind(mailSettings);

            builder.RegisterModule(new MediatorModule());

            builder.RegisterModule(new ApplicationModule(Configuration[OutgoingCommunicationsSettingsKeys.ConnectionString], mailSettings, Configuration[OutgoingCommunicationsSettingsKeys.EmailsTemplatesDirectoryBasePath], Configuration[OutgoingCommunicationsSettingsKeys.ImagesBaseUrl]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ConfigureEventBus(app);
        }

        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddTransient<NewsletterSubscriptionSubmittedIntegrationEventHandler>();
            services.AddTransient<NewsletterSubscriptionConfirmedIntegrationEventHandler>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<NewsletterSubscriptionSubmittedIntegrationEvent, NewsletterSubscriptionSubmittedIntegrationEventHandler>();
            eventBus.Subscribe<NewsletterSubscriptionConfirmedIntegrationEvent, NewsletterSubscriptionConfirmedIntegrationEventHandler>();
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(configuration[OutgoingCommunicationsSettingsKeys.ConnectionString],
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
            });

            return services;
        }
    }
}