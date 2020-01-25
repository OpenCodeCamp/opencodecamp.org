namespace OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate
{
    using OpenCodeCamp.Services.Marketing.Domain.Seedwork;
    using System.Threading.Tasks;

    public interface INewsletterSubscriptionRepository : IRepository<NewsletterSubscription>
    {
        NewsletterSubscription Add(NewsletterSubscription newsletterSubscription);

        void Update(NewsletterSubscription newsletterSubscription);

        Task<NewsletterSubscription> GetAsync(int id);

        Task<NewsletterSubscription> GetAsync(string email);

        Task<bool> IsEmailAlreadyUsesAsync(string email);
    }
}
