namespace OpenCodeCamp.Services.Marketing.Api.Application.Validations
{
    using FluentValidation;
    using Microsoft.Extensions.Logging;
    using OpenCodeCamp.Services.Marketing.Api.Application.Commands;

    public class NewsletterSubscriptionConfirmationCommandValidator : AbstractValidator<NewsletterSubscriptionConfirmationCommand>
    {
        public NewsletterSubscriptionConfirmationCommandValidator(ILogger<NewsletterSubscriptionConfirmationCommandValidator> logger)
        {
            RuleFor(command => command.EmailAddress).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}