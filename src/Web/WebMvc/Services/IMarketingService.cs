using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenCodeCamp.WebMvc.Services
{
    public interface IMarketingService
    {
        Task SubcribeToNewsletter(string email);
        Task ConfirmNewsletterSubscription(string email, string confirmationToken);
    }
}
