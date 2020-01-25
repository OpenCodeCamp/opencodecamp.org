namespace OpenCodeCamp.Services.Marketing.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using OpenCodeCamp.BuildingBlocks.EventBus.Extensions;
    using OpenCodeCamp.Services.Marketing.Api.Application.Commands;
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure.Services;
    using Microsoft.Extensions.Logging;
    using Marketing.Api.Application.Behaviors;
    using System.Net;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class NewsletterSubscriptionsController : Controller
    {
        private readonly IMediator _mediator;
        //private readonly IOrderQueries _orderQueries;
        private readonly IIdentityService _identityService;
        private readonly ILogger<NewsletterSubscriptionsController> _logger;

        public NewsletterSubscriptionsController(
            IMediator mediator,
            IIdentityService identityService,
            ILogger<NewsletterSubscriptionsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("create")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> SubscribeToNewsletterAsync([FromBody] SubscribeToNewsletterCommand command
            , [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var subscribeToNewsletterCommand = new IdentifiedCommand<SubscribeToNewsletterCommand, bool>(command, guid);

                this._logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                subscribeToNewsletterCommand.GetGenericTypeName(),
                nameof(subscribeToNewsletterCommand.Command.EmailAddress),
                subscribeToNewsletterCommand.Command.EmailAddress,
                subscribeToNewsletterCommand);

                commandResult = await this._mediator.Send(subscribeToNewsletterCommand);
            }

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Route("confirm")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ConfirmAsync([FromBody] NewsletterSubscriptionConfirmationCommand command
            , [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var newsletterSubscriptionConfirmationCommand = new
                    IdentifiedCommand<NewsletterSubscriptionConfirmationCommand, bool>(command, guid);

                this._logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                newsletterSubscriptionConfirmationCommand.GetGenericTypeName(),
                nameof(newsletterSubscriptionConfirmationCommand.Command.EmailAddress),
                newsletterSubscriptionConfirmationCommand.Command.EmailAddress,
                newsletterSubscriptionConfirmationCommand);

                commandResult = await this._mediator.Send(newsletterSubscriptionConfirmationCommand);
            }

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}