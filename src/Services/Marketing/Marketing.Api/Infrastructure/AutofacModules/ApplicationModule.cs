namespace OpenCodeCamp.Services.Marketing.Api.Infrastructure.AutofacModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    using Autofac;
    using OpenCodeCamp.Services.Marketing.Infrastructure.Idempotency;
    using OpenCodeCamp.BuildingBlocks.EventBus.Abstractions;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using OpenCodeCamp.Services.Marketing.Infrastructure.Repositories;

    public class ApplicationModule
        : Autofac.Module
    {
        public string QueriesConnectionString { get; }

        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NewsletterSubscriptionRepository>()
                .As<INewsletterSubscriptionRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();
        }
    }
}