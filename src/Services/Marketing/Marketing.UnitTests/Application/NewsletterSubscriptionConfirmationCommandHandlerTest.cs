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
    using OpenCodeCamp.Services.Marketing.Domain.Exceptions;

    public class NewsletterSubscriptionConfirmationCommandHandlerTest
    {
        private readonly Mock<INewsletterSubscriptionRepository> _newsletterSubscriptionRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IMarketingIntegrationEventService> _marketingIntegrationEventService;
        private readonly Mock<ILogger<NewsletterSubscriptionConfirmationCommandHandler>> _loggerMock;

        public NewsletterSubscriptionConfirmationCommandHandlerTest()
        {
            this._newsletterSubscriptionRepositoryMock = new Mock<INewsletterSubscriptionRepository>();
            this._identityServiceMock = new Mock<IIdentityService>();
            this._marketingIntegrationEventService = new Mock<IMarketingIntegrationEventService>();
            this._mediator = new Mock<IMediator>();
            this._loggerMock = new Mock<ILogger<NewsletterSubscriptionConfirmationCommandHandler>>();
        }

        [Fact]
        public async Task Handle_return_exception_if_subscription_is_not_exists()
        {
            // Arrange
            var fakeCommand = this.FakeNewsletterSubscriptionConfirmationCommand();

            this._newsletterSubscriptionRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<int>()))
               .Returns(Task.FromResult<NewsletterSubscription>(null));

            this._newsletterSubscriptionRepositoryMock.Setup(repo => repo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            var LoggerMock = new Mock<ILogger<SubscribeToNewsletterCommandHandler>>();

            // Act
            var handler = new NewsletterSubscriptionConfirmationCommandHandler(this._mediator.Object,
                this._marketingIntegrationEventService.Object, this._newsletterSubscriptionRepositoryMock.Object, this._loggerMock.Object);
            var cltToken = new System.Threading.CancellationToken();

            // Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(fakeCommand, cltToken));
        }

        [Fact]
        public async Task Handle_return_exception_if_subscription_is_confirmed()
        {
            // Arrange
            var fakeCommand = this.FakeNewsletterSubscriptionConfirmationCommand();

            var newsletterSubscription = this.FakeNewsletterSubscription();
            newsletterSubscription.Confirm(this.Token);

            this._newsletterSubscriptionRepositoryMock.Setup(repo => repo.GetAsync(this.Email))
               .Returns(Task.FromResult<NewsletterSubscription>(newsletterSubscription));

            this._newsletterSubscriptionRepositoryMock.Setup(repo => repo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            var LoggerMock = new Mock<ILogger<SubscribeToNewsletterCommandHandler>>();

            // Act
            var handler = new NewsletterSubscriptionConfirmationCommandHandler(this._mediator.Object,
                this._marketingIntegrationEventService.Object, this._newsletterSubscriptionRepositoryMock.Object, this._loggerMock.Object);
            var cltToken = new System.Threading.CancellationToken();

            // Assert
            await Assert.ThrowsAsync<MarketingDomainException>(() => handler.Handle(fakeCommand, cltToken));
        }

        [Fact]
        public async Task Handle_return_exception_if_token_already_used()
        {
            // Arrange
            var fakeCommand = this.FakeNewsletterSubscriptionConfirmationCommand();

            var newsletterSubscription = this.FakeNewsletterSubscription();
            newsletterSubscription.Tokens.Single().Use();

            this._newsletterSubscriptionRepositoryMock.Setup(repo => repo.GetAsync(this.Email))
               .Returns(Task.FromResult<NewsletterSubscription>(newsletterSubscription));

            this._newsletterSubscriptionRepositoryMock.Setup(repo => repo.UnitOfWork.SaveChangesAsync(default(CancellationToken)))
                .Returns(Task.FromResult(1));

            var LoggerMock = new Mock<ILogger<SubscribeToNewsletterCommandHandler>>();

            // Act
            var handler = new NewsletterSubscriptionConfirmationCommandHandler(this._mediator.Object,
                this._marketingIntegrationEventService.Object, this._newsletterSubscriptionRepositoryMock.Object, this._loggerMock.Object);
            var cltToken = new System.Threading.CancellationToken();

            // Assert
            await Assert.ThrowsAsync<MarketingDomainException>(() => handler.Handle(fakeCommand, cltToken));
        }

        private string Email => FakeDataProvider.valid_email_address;
        private string Language => FakeDataProvider.valid_english_language_code;
        private string Token => Guid.NewGuid().ToString().Replace("-", String.Empty);

        private NewsletterSubscriptionConfirmationCommand FakeNewsletterSubscriptionConfirmationCommand() =>
            new NewsletterSubscriptionConfirmationCommand(this.Language, this.Email, this.Token);

        private NewsletterSubscription FakeNewsletterSubscription() =>
            new NewsletterSubscription(this.Language, this.Email);
    }
}