namespace OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.EventHandling
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenCodeCamp.BuildingBlocks.EventBus.Abstractions;
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;
    using OpenCodeCamp.OutgoingCommunications.Emails;
    using OpenCodeCamp.Services.OutgoingCommunications.Api.Infrastructure.Configuration;
    using OpenCodeCamp.Services.OutgoingCommunications.Api.IntegrationEvents.Events;
    using OpenCodeCamp.Services.OutgoingCommunications.Api.Services;
    using Serilog.Context;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Handle the event sent by the Marketing when a new newsletter subscription is submitted.
    /// Our business case is to send the mail to the user to allow him to confirm his subscription.
    /// </summary>
    public class NewsletterSubscriptionConfirmedIntegrationEventHandler
        : IIntegrationEventHandler<NewsletterSubscriptionConfirmedIntegrationEvent>
    {
        private readonly IEmailSender emailSender;
        private readonly IEventBus _eventBus;
        private readonly OutgoingCommunicationsSettings _settings;
        private readonly ILogger<NewsletterSubscriptionConfirmedIntegrationEventHandler> _logger;
        private readonly IEmailBuilderService _emailBuilder;

        public NewsletterSubscriptionConfirmedIntegrationEventHandler(IEmailSender emailSender,
            IEventBus eventBus,
            IOptionsSnapshot<OutgoingCommunicationsSettings> settings,
            ILogger<NewsletterSubscriptionConfirmedIntegrationEventHandler> logger,
            IEmailBuilderService emailBuilder)
        {
            this.emailSender = emailSender;
            _eventBus = eventBus;
            _settings = settings.Value;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this._emailBuilder = emailBuilder ?? throw new System.ArgumentNullException(nameof(emailBuilder));

            _logger.LogTrace("OutgoingCommunicationsSettings: {@OutgoingCommunicationsSettings}", _settings);
        }

        public async Task Handle(NewsletterSubscriptionConfirmedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                // Business feature comment:
                // We send a mail
                //_logger.LogInformation("----- Mail sending status: " + mailSent);
                Languages lang = @event.Language == "fr" || @event.Language == "fr-fr" || @event.Language == "fr-FR" ? Languages.Fr : Languages.En;
                EmailContent emailContent = await this._emailBuilder.GetNewsletterSubscriptionConfirmedEmailContentAsync(lang);
                bool mailSent = await this.emailSender.SendEmailAsync(
                                    @event.EmailAddress,
                                    emailContent.Subject,
                                    emailContent.BodyHtmlContent,
                                    true);

                _logger.LogInformation("----- Mail sending status: " + mailSent);

                //Business feature comment:
                // We send an integration event to say if the mail is sent or not.
                IntegrationEvent mailSendingStatusIntegrationEvent;
                if (mailSent)
                {
                    mailSendingStatusIntegrationEvent = new NewsletterSubscriptionConfirmedEmailSuccededIntegrationEvent(
                        @event.EmailAddress, DateTime.Now);
                }
                else
                {
                    mailSendingStatusIntegrationEvent = new NewsletterSubscriptionConfirmedEmailFailedIntegrationEvent(@event.EmailAddress);
                }

                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                    mailSendingStatusIntegrationEvent.Id, Program.AppName, mailSendingStatusIntegrationEvent);

                _eventBus.Publish(mailSendingStatusIntegrationEvent);


                await Task.CompletedTask;
            }
        }
    }
}