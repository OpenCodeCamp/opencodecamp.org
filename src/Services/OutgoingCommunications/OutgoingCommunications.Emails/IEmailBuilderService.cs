namespace OpenCodeCamp.OutgoingCommunications.Emails
{
    using System.Threading.Tasks;

    public interface IEmailBuilderService
    {
        Task<EmailContent> GetConfirmNewsletterSubscriptionEmailContentAsync(Languages lang, string confirmUrl);
        Task<EmailContent> GetNewsletterSubscriptionConfirmedEmailContentAsync(Languages lang);
    }
}