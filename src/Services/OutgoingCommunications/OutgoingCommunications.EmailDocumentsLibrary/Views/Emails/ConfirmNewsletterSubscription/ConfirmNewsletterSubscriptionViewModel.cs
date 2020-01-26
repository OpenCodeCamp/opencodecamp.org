using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenCodeCamp.OutgoingCommunications.EmailDocumentsLibrary.Views.Emails.ConfirmNewsletterSubscription
{
    public sealed class ConfirmNewsletterSubscriptionViewModel
    {
        public ConfirmNewsletterSubscriptionViewModel(string confirmEmailUrl)
        {
            ConfirmEmailUrl = confirmEmailUrl;
        }

        public string ConfirmEmailUrl { get; set; }
    }
}
