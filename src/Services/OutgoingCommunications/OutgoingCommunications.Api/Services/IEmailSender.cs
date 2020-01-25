namespace OpenCodeCamp.Services.OutgoingCommunications.Api.Services
{
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(string recipientMailAddress, string subject, string body, bool isBodyHtml);
    }
}