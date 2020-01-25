namespace OpenCodeCamp.Services.Marketing.Api.Application.Validations
{
    using FluentValidation;
    using OpenCodeCamp.Services.Marketing.Api.Application.Commands;
    using Microsoft.Extensions.Logging;

    public class IdentifiedCommandValidator : AbstractValidator<IdentifiedCommand<SubscribeToNewsletterCommand, bool>>
    {
        public IdentifiedCommandValidator(ILogger<IdentifiedCommandValidator> logger)
        {
            RuleFor(command => command.Id).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}