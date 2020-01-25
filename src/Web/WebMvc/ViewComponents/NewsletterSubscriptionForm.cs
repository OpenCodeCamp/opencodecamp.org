namespace OpenCodeCamp.WebMvc.ViewComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using OpenCodeCamp.WebMvc.Services;
    using OpenCodeCamp.WebMvc.ViewModels.NewsletterSubscriptionViewModels;

    public class NewsletterSubscriptionForm : ViewComponent
    {
        private readonly IMarketingService _marketingSvc;

        public NewsletterSubscriptionForm(IMarketingService marketingSvc) => _marketingSvc = marketingSvc;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new NewsletterSubscriptionFormViewModel();

            return View(vm);
        }

        public async Task OnSubmitAsync()
        {

        }
    }
}
