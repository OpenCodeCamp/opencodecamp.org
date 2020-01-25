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

    public class NewsletterSubscriptionsWebApiTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<ILogger<NewsletterSubscriptionsController>> _loggerMock;

        public NewsletterSubscriptionsWebApiTest()
        {
            this._mediatorMock = new Mock<IMediator>();
            this._identityServiceMock = new Mock<IIdentityService>();
            this._loggerMock = new Mock<ILogger<NewsletterSubscriptionsController>>();
        }

        private NewsletterSubscriptionsController BuildValidNewsletterSubscriptionsController() =>
            new NewsletterSubscriptionsController(this._mediatorMock.Object, this._identityServiceMock.Object, this._loggerMock.Object);

        #region Subscribe to newsletter tests

        [Fact]
        public async Task Subscribe_to_newsletter_with_requestId_success()
        {
            // Arrange
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<IdentifiedCommand<SubscribeToNewsletterCommand, bool>>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(true));
            string valid_email = FakeDataProvider.valid_email_address;
            string valid_lang = FakeDataProvider.valid_english_language_code;
            var command = new SubscribeToNewsletterCommand(valid_lang, valid_email);
            Guid requestId = Guid.NewGuid();

            // Act
            var newsletterSubscriptionsController = this.BuildValidNewsletterSubscriptionsController();
            var actionResult = await newsletterSubscriptionsController.SubscribeToNewsletterAsync(command, requestId.ToString()) as OkResult;

            // Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Subscribe_to_newsletter_with_no_requestId_bad_request()
        {
            // Arrange
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<IdentifiedCommand<SubscribeToNewsletterCommand, bool>>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(true));
            string valid_email = FakeDataProvider.valid_email_address;
            string valid_lang = FakeDataProvider.valid_english_language_code;
            var command = new SubscribeToNewsletterCommand(valid_lang, valid_email);

            // Act
            var newsletterSubscriptionsController = this.BuildValidNewsletterSubscriptionsController();
            var actionResult = await newsletterSubscriptionsController.SubscribeToNewsletterAsync(command, string.Empty) as BadRequestResult;

            // Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        #endregion Subscribe to newsletter tests

        #region Confirm newsletter subscription tests

        private NewsletterSubscriptionConfirmationCommand validNewsletterSubscriptionConfirmationCommand
        {
            get
            {
                string valid_email = FakeDataProvider.valid_email_address;
                string valid_lang = FakeDataProvider.valid_english_language_code;
                string valid_token = FakeDataProvider.NewsletterSubscriptionFakeDataProvider.confirmation_token;
                var command = new NewsletterSubscriptionConfirmationCommand(valid_lang, valid_email, valid_token);
                return command;
            }
        }

        [Fact]
        public async Task Confirm_newsletter_subscription_with_requestId_success()
        {
            // Arrange
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<IdentifiedCommand<NewsletterSubscriptionConfirmationCommand, bool>>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(true));
            var command = this.validNewsletterSubscriptionConfirmationCommand;
            Guid requestId = Guid.NewGuid();

            // Act
            var newsletterSubscriptionsController = this.BuildValidNewsletterSubscriptionsController();
            var actionResult = await newsletterSubscriptionsController.ConfirmAsync(command, requestId.ToString()) as OkResult;

            // Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Confirm_newsletter_subscription_with_no_requestId_bad_request()
        {
            // Arrange
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<IdentifiedCommand<NewsletterSubscriptionConfirmationCommand, bool>>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(true));
            var command = this.validNewsletterSubscriptionConfirmationCommand;

            // Act
            var newsletterSubscriptionsController = this.BuildValidNewsletterSubscriptionsController();
            var actionResult = await newsletterSubscriptionsController.ConfirmAsync(command, string.Empty) as BadRequestResult;

            // Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        #endregion Confirm newsletter subscription tests
    }
}