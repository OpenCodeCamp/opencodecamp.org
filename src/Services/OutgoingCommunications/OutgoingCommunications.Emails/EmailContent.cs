namespace OpenCodeCamp.OutgoingCommunications.Emails
{
    using System;

    public sealed class EmailContent
    {
        public Languages Language { get; }
        public string BodyHtmlContent { get; }
        public string Subject { get; }

        public EmailContent(Languages lang, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentNullException(nameof(body));
            }

            this.Language = lang;
            this.Subject = subject;
            this.BodyHtmlContent = body;
        }
    }
}