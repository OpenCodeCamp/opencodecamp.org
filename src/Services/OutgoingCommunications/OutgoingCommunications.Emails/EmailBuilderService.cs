namespace OpenCodeCamp.OutgoingCommunications.Emails
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Linq;

    public sealed class EmailBuilderService : IEmailBuilderService
    {
        private readonly string documentsBaseDirectory;
        private readonly string imagesBaseUrl;
        private const string headerFilePath = "Shared/_Header.html";
        private const string footerFilePath = "Shared/_Footer.html";
        private const string layoutFilePath = "Shared/_Layout.html";

        public EmailBuilderService(string documentsBaseDirectory, string imagesBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(documentsBaseDirectory))
            {
                throw new ArgumentNullException(nameof(documentsBaseDirectory));
            }

            if (string.IsNullOrWhiteSpace(imagesBaseUrl))
            {
                throw new ArgumentNullException(nameof(imagesBaseUrl));
            }

            this.documentsBaseDirectory = documentsBaseDirectory;
            this.imagesBaseUrl = imagesBaseUrl;
        }

        public class EmailMetadata
        {
            [JsonPropertyName("languages")]
            public ICollection<EmailMetadataLanguageDetail> Languages { get; set; }
        }

        public class EmailMetadataLanguageDetail
        {
            [JsonPropertyName("lang")]
            public string Language { get; set; }

            [JsonPropertyName("object")]
            public string MailObject { get; set; }
        }

        private EmailMetadata LoadMetadata(string emailDirectoryName)
        {
            if (string.IsNullOrWhiteSpace(emailDirectoryName))
            {
                throw new ArgumentNullException(nameof(emailDirectoryName));
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            string jsonString = File.ReadAllText(Path.Combine(documentsBaseDirectory, "ConfirmNewsletterSubscription", "metadata.json"));
            return JsonSerializer.Deserialize<EmailMetadata>(jsonString, options);
        }

        private class EmailLayoutContents
        {
            public string Content { get; }
            public string Subject { get; }

            internal EmailLayoutContents(string subject, string content)
            {
                this.Subject = subject;
                this.Content = content;
            }
        }

        private async Task<EmailLayoutContents> LoadEmailLayoutContentsAsync(Languages lang, string emailDirectoryName)
        {
            if (string.IsNullOrWhiteSpace(emailDirectoryName))
            {
                throw new ArgumentNullException(nameof(emailDirectoryName));
            }

            string headerContent = File.ReadAllText(Path.Combine(documentsBaseDirectory, headerFilePath));
            string footerContent = File.ReadAllText(Path.Combine(documentsBaseDirectory, footerFilePath));
            string layoutContent = File.ReadAllText(Path.Combine(documentsBaseDirectory, layoutFilePath));

            string mailContent = File.ReadAllText(Path.Combine(documentsBaseDirectory, emailDirectoryName, "_Content." + lang + ".html"));

            var metadata = this.LoadMetadata(emailDirectoryName);
            string subject = metadata.Languages.Single(x => x.Language == lang.ToString().ToLower()).MailObject;

            string completeMailContent = layoutContent.Replace("||[_Header]||", headerContent)
                .Replace("||[_Footer]||", footerContent)
                .Replace("||[_Content]||", mailContent);

            return new EmailLayoutContents(subject, completeMailContent);
        }

        public async Task<EmailContent> GetConfirmNewsletterSubscriptionEmailContentAsync(Languages lang, string confirmUrl)
        {
            if (string.IsNullOrWhiteSpace(confirmUrl))
            {
                throw new ArgumentNullException(nameof(confirmUrl));
            }

            EmailLayoutContents emailContent = await this.LoadEmailLayoutContentsAsync(lang, "ConfirmNewsletterSubscription");

            string bodyContent = emailContent.Content
                                    .Replace("%%ConfirmEmailUrl%%", confirmUrl)
                                    .Replace("%%ImagesBaseUrl%%", this.imagesBaseUrl);

            EmailContent email = new EmailContent(lang, emailContent.Subject, bodyContent);
            return email;
        }

        public async Task<EmailContent> GetNewsletterSubscriptionConfirmedEmailContentAsync(Languages lang)
        {
            EmailLayoutContents emailContent = await this.LoadEmailLayoutContentsAsync(lang, "NewsletterSubscriptionConfirmed");

            string bodyContent = emailContent.Content
                                    .Replace("%%ImagesBaseUrl%%", this.imagesBaseUrl);

            EmailContent email = new EmailContent(lang, emailContent.Subject, bodyContent);
            return email;
        }
    }
}