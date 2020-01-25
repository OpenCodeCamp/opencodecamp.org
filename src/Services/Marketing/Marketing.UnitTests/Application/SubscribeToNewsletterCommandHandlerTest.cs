namespace UnitTest.Marketing.Application
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using OpenCodeCamp.Services.Marketing.Api.Controllers;
    using OpenCodeCamp.Services.Marketing.Api.Application.Commands;
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure.Services;
    using OpenCodeCamp.Services.Marketing.Infrastructure.Idempotency;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using OpenCodeCamp.Services.Marketing.Api.Application.IntegrationEvents;

    public class SubscribeToNewsletterCommandHandlerTest
    {
        private readonly Mock<INewsletterSubscriptionRepository> _newsletterSubscriptionRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IMarketingIntegrationEventService> _marketingIntegrationEventService;

        public SubscribeToNewsletterCommandHandlerTest()
        {
            this._newsletterSubscriptionRepositoryMock = new Mock<INewsletterSubscriptionRepository>();
            this._identityServiceMock = new Mock<IIdentityService>();
            this._marketingIntegrationEventService = new Mock<IMarketingIntegrationEventService>();
            this._mediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Handle_return_false_if_subscription_is_not_persisted()
        {
            // Arrange
            var fakeCommand = this.FakeSubscribeToNewsletterCommand();

            this._newsletterSubscriptionRepositoryMock.Setup(repo => repo.Add(It.IsAny<NewsletterSubscription>()))
               .Returns(this.FakeNewsletterSubscription());

            this._newsletterSubscriptionRepositoryMock.Setup(repo => repo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            var LoggerMock = new Mock<ILogger<SubscribeToNewsletterCommandHandler>>();

            // Act
            var handler = new SubscribeToNewsletterCommandHandler(this._mediator.Object, this._marketingIntegrationEventService.Object,
                this._newsletterSubscriptionRepositoryMock.Object, LoggerMock.Object);
            var cltToken = new System.Threading.CancellationToken();
            var result = await handler.Handle(fakeCommand, cltToken);

            // Assert
            Assert.False(result);
        }

        private NewsletterSubscription FakeNewsletterSubscription() =>
            new NewsletterSubscription(FakeDataProvider.valid_english_language_code, FakeDataProvider.valid_email_address);

        private SubscribeToNewsletterCommand FakeSubscribeToNewsletterCommand() =>
            new SubscribeToNewsletterCommand(FakeDataProvider.valid_english_language_code, FakeDataProvider.valid_email_address);
    }
}