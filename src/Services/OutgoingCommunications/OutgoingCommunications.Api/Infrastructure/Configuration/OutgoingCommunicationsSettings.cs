namespace OpenCodeCamp.Services.OutgoingCommunications.Api.Infrastructure.Configuration
{
    public class OutgoingCommunicationsSettings
    {
        public string ConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public MailSettings Mail { get; set; }

        public string EmailsTemplatesDirectoryBasePath { get; set; }

        public string ImagesBaseUrl { get; set; }
    }
}