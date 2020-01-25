namespace OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate
{
    using Marketing.Domain.Exceptions;
    using OpenCodeCamp.Services.Marketing.Domain.Seedwork;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NewsletterSubscriptionStatus
         : Enumeration
    {
        public static NewsletterSubscriptionStatus Submitted = new NewsletterSubscriptionStatus(1, nameof(Submitted).ToLowerInvariant());
        public static NewsletterSubscriptionStatus Confirmed = new NewsletterSubscriptionStatus(2, nameof(Confirmed).ToLowerInvariant());
        public static NewsletterSubscriptionStatus AwaitingCancellationValidation = new NewsletterSubscriptionStatus(3, nameof(AwaitingCancellationValidation).ToLowerInvariant());
        public static NewsletterSubscriptionStatus Cancelled = new NewsletterSubscriptionStatus(4, nameof(Cancelled).ToLowerInvariant());

        public NewsletterSubscriptionStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<NewsletterSubscriptionStatus> List() =>
            new[] { Submitted, Confirmed, AwaitingCancellationValidation, Cancelled };

        public static NewsletterSubscriptionStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new MarketingDomainException($"Possible values for NewsletterSubscriptionStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static NewsletterSubscriptionStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new MarketingDomainException($"Possible values for NewsletterSubscriptionStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}