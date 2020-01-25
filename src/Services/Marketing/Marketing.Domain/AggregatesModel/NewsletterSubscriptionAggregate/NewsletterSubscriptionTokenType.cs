namespace OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate
{
    using OpenCodeCamp.Services.Marketing.Domain.Seedwork;

    public class NewsletterSubscriptionTokenType
          : Enumeration
    {
        public static NewsletterSubscriptionTokenType Confirmation = new NewsletterSubscriptionTokenType(1, "Confirmation");
        public static NewsletterSubscriptionTokenType Cancellation = new NewsletterSubscriptionTokenType(2, "Cancellation");

        public NewsletterSubscriptionTokenType(int id, string name)
            : base(id, name)
        {
        }
    }
}