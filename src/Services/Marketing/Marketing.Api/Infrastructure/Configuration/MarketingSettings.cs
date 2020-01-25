namespace OpenCodeCamp.Services.Marketing.Api.Infrastructure.Configuration
{
    public class MarketingSettings
    {
        public string ConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public int CheckUpdateTime { get; set; }
        public string IdentityUrl { get; set; }
        public string WebFrontOfficeUrl { get; set; }
    }
}
