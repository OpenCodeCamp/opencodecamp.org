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

    public class IdentifiedCommandHandlerTest
    {
        private readonly Mock<IRequestManager> _requestManager;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<ILogger<IdentifiedCommandHandler<NewsletterSubscriptionConfirmationCommand, bool>>> _confirmationLoggerMock;

        public IdentifiedCommandHandlerTest()
        {
            this._requestManager = new Mock<IRequestManager>();
            this._mediator = new Mock<IMediator>();
            this._confirmationLoggerMock = new Mock<ILogger<IdentifiedCommandHandler<NewsletterSubscriptionConfirmationCommand, bool>>>();
        }

        [Fact]
        public async Task Handle_return_false_when_subscription_is_not_exist()
        {
            // Arrange
            var fakeGuid = Guid.NewGuid();
            var fakeCommand = new IdentifiedCommand<NewsletterSubscriptionConfirmationCommand, bool>(this.FakeNewsletterSubscriptionConfirmationCommand(), fakeGuid);

            _requestManager.Setup(x => x.ExistAsync(It.IsAny<Guid>()))
               .Returns(Task.FromResult(false));

            _mediator.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), default(System.Threading.CancellationToken)))
               .Returns(Task.FromResult(true));

            // Act
            var handler = new IdentifiedCommandHandler<NewsletterSubscriptionConfirmationCommand, bool>(
                this._mediator.Object, this._requestManager.Object, this._confirmationLoggerMock.Object);
            var cltToken = new System.Threading.CancellationToken();
            var result = await handler.Handle(fakeCommand, cltToken);

            // Assert
            Assert.True(result);
            _mediator.Verify(x => x.Send(It.IsAny<IRequest<bool>>(), default(System.Threading.CancellationToken)), Times.Once());
        }

        private string Email => FakeDataProvider.valid_email_address;
        private string Language => FakeDataProvider.valid_english_language_code;
        private string Token => Guid.NewGuid().ToString().Replace("-", String.Empty);

        private NewsletterSubscriptionConfirmationCommand FakeNewsletterSubscriptionConfirmationCommand() =>
            new NewsletterSubscriptionConfirmationCommand(this.Language, this.Email, this.Token);
    }
}