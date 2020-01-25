namespace OpenCodeCamp.Services.Marketing.Api.Application.Validations
{
    using FluentValidation;
    using Microsoft.Extensions.Logging;
    using OpenCodeCamp.Services.Marketing.Api.Application.Commands;

    public class SubscribeToNewsletterCommandValidator : AbstractValidator<SubscribeToNewsletterCommand>
    {
        public SubscribeToNewsletterCommandValidator(ILogger<SubscribeToNewsletterCommandValidator> logger)
        {
            RuleFor(command => command.EmailAddress).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}