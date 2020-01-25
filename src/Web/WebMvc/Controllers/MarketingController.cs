using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenCodeCamp.WebMvc.Services;
using OpenCodeCamp.WebMvc.ViewModels.NewsletterSubscriptionViewModels;

namespace OpenCodeCamp.WebMvc.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class MarketingController : Controller
    {
        private readonly ILogger<MarketingController> logger;
        private readonly IMarketingService marketingService;

        public MarketingController(ILogger<MarketingController> logger, IMarketingService marketingService)
        {
            this.logger = logger;
            this.marketingService = marketingService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SubscribeToNewsletter(NewsletterSubscriptionFormViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var culture = System.Globalization.CultureInfo.CurrentCulture;
                var uiCulture = System.Globalization.CultureInfo.CurrentUICulture;
                await this.marketingService.SubcribeToNewsletter(vm.EmailAddress);

                return Ok();
                //return Microsoft.AspNetCore.Http.StatusCodes.Status201Created;
                //return Created();
            }
            else
            {
                ModelState.AddModelError("Error", "Une erreur s'est produite");
            }

            //return BadRequest();
            //return BadRequest(ModelState);
            return View("_NewsletterSubscription", vm);
        }

        [HttpGet]
        //[Route("confirm-newsletter-subscription/{email:length(10,60):required}/{token:length(32):required}")]
        public async Task<IActionResult> ConfirmNewsletterSubscription([FromRoute]string email, [FromRoute]string token)
        {
            this.logger.LogInformation("Entry in ConfirmNewsletterSubscription");
            this.logger.LogInformation("Email:" + email);
            this.logger.LogInformation("Token:" + token);

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (email.Length < 10 || email.Length > 60)
            {
                throw new ArgumentException(nameof(email));
            }

            if (token.Length != 32)
            {
                throw new ArgumentException(nameof(token));
            }

            if (ModelState.IsValid)
            {
                await this.marketingService.ConfirmNewsletterSubscription(email, token);

                return View("NewsletterSubscriptionConfirmed");
            }

            return RedirectToAction(nameof(HomeController.Error), "Home");
        }

        //// Temporary method
        //public string ShowMeCulture()
        //{
        //    return $"CurrentCulture:{System.Globalization.CultureInfo.CurrentCulture.Name}, CurrentUICulture:{System.Globalization.CultureInfo.CurrentUICulture.Name}";
        //}

        //[HttpGet]
        //public string test()
        //{
        //    return "test";
        //}
    }
}