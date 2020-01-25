namespace OpenCodeCamp.Services.OutgoingCommunications.Api.Infrastructure.Configuration
{
    public class MailSettings
    {
        public string From { get; set; }
        public string CCs { get; set; }
        public SMTP SMTP { get; set; }
        public bool IsServiceEnabled { get; set; }
    }

    public class SMTP
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public NetworkCredentials NetworkCredential { get; set; }
    }

    public class NetworkCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
