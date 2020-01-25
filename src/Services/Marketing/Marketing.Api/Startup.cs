namespace OpenCodeCamp.Services.Marketing.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Http;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using global::OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents;
    using global::OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents.Events;
    using global::OpenCodeCamp.Services.Marketing.Api.Infrastructure.Filters;
    using global::OpenCodeCamp.Services.Marketing.Api.Infrastructure.Middlewares;
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure.AutofacModules;
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure.Services;
    using Microsoft.EntityFrameworkCore;
    using OpenCodeCamp.BuildingBlocks.EventBus;
    using OpenCodeCamp.BuildingBlocks.EventBus.Abstractions;
    using OpenCodeCamp.BuildingBlocks.EventBusRabbitMQ;
    using OpenCodeCamp.BuildingBlocks.EventBusServiceBus;
    using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF;
    using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF.Services;
    using OpenCodeCamp.Services.Marketing.Infrastructure;
    using RabbitMQ.Client;
    using System.Data.Common;
    using System.Reflection;
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure.Configuration;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // It's virtual to can be overrided for the functionnal unit tests.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MarketingSettings>(Configuration);

            services
                .AddCustomDbContext(Configuration)
                .AddCustomIntegrations(Configuration)
                .AddEventBus(Configuration)
                .AddControllers();
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you. If you
        // need a reference to the container, you need to use the
        // "Without ConfigureContainer" mechanism shown later.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<BuildingBlocks.EventBus.Abstractions.IEventBus>();

            //eventBus.Subscribe<NewsletterSubscriptionSubmittedIntegrationEvent, IIntegrationEventHandler<NewsletterSubscriptionSubmittedIntegrationEvent>>();
            //eventBus.Subscribe<NewsletterSubscriptionConfirmedIntegrationEvent, IIntegrationEventHandler<NewsletterSubscriptionConfirmedIntegrationEvent>>();
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<MarketingContext>(options =>
                   {
                       options.UseSqlServer(configuration["ConnectionString"],
                           sqlServerOptionsAction: sqlOptions =>
                           {
                               sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                               sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                           });
                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );

            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"],
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
            });

            return services;
        }


        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddTransient<IMarketingIntegrationEventService, MarketingIntegrationEventService>();


            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrWhiteSpace(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrWhiteSpace(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrWhiteSpace(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];


            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrWhiteSpace(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
    }
}