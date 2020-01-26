namespace OpenCodeCamp.Services.OutgoingCommunications.Api.Services
{
    using OpenCodeCamp.Services.OutgoingCommunications.Api.Infrastructure.Configuration;
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public sealed class EmailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailSender(MailSettings mailSettings)
        {
            _mailSettings = mailSettings ?? throw new ArgumentNullException(nameof(mailSettings));
        }

        public async Task<bool> SendEmailAsync(string recipientMailAddress, string subject, string body, bool isBodyHtml)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(_mailSettings.SMTP.Host))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_mailSettings.SMTP.NetworkCredential.Username, _mailSettings.SMTP.NetworkCredential.Password);
                    client.Port = _mailSettings.SMTP.Port;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(_mailSettings.From);
                        mailMessage.To.Add(recipientMailAddress);
                        mailMessage.Body = body;
                        mailMessage.Subject = subject;
                        mailMessage.IsBodyHtml = isBodyHtml;
                        if (!string.IsNullOrWhiteSpace(_mailSettings.CCs))
                        {
                            mailMessage.To.Add(_mailSettings.CCs);
                        }

                        await client.SendMailAsync(mailMessage);

                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}