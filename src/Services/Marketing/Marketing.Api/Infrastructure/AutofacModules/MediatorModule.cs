namespace OpenCodeCamp.Services.Marketing.Api.Infrastructure.AutofacModules
{
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using FluentValidation;
    using MediatR;
    using OpenCodeCamp.Services.Marketing.Api.Application.Commands;
    using OpenCodeCamp.Services.Marketing.Api.Application.Behaviors;
    using OpenCodeCamp.Services.Marketing.Api.Application.Validations;
    using OpenCodeCamp.Services.Marketing.Api.Application.DomainEventHandlers.NewsletterSubscriptionSubmitted;
    using OpenCodeCamp.Services.Marketing.Api.Application.DomainEventHandlers.NewsletterSubscriptionConfirmed;

    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(typeof(SubscribeToNewsletterCommand).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));
            builder.RegisterAssemblyTypes(typeof(NewsletterSubscriptionConfirmationCommand).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
            builder.RegisterAssemblyTypes(typeof(NewsletterSubscriptionSubmittedDomainEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));
            builder.RegisterAssemblyTypes(typeof(NewsletterSubscriptionConfirmedDomainEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register the Command's Validators (Validators based on FluentValidation library)
            builder
                .RegisterAssemblyTypes(typeof(SubscribeToNewsletterCommandValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            builder
                .RegisterAssemblyTypes(typeof(NewsletterSubscriptionConfirmationCommandValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();


            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));

        }
    }
}