namespace OpenCodeCamp.WebMvc.Services
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using OpenCodeCamp.WebMvc.Infrastructure;
    using OpenCodeCamp.WebMvc.Services.ModelDtos;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public sealed class MarketingService : IMarketingService
    {
        private HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly IOptions<AppSettings> _settings;

        public MarketingService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings;

            _remoteServiceBaseUrl = $"{settings.Value.MarketingUrl}/api/v1";
        }

        async public Task SubcribeToNewsletter(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            string uri = Api.MarketingApi.CreateNewsletterSubscription(_remoteServiceBaseUrl);

            var dto = new SubscribeToNewsletterCommand(CultureInfo.CurrentCulture, email);

            var bodyContent = new StringContent(JsonConvert.SerializeObject(dto), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, bodyContent);

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception("Error in newsletter subscription process, try later.");
            }

            response.EnsureSuccessStatusCode();
        }

        async public Task ConfirmNewsletterSubscription(string email, string confirmationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(confirmationToken))
            {
                throw new ArgumentNullException(nameof(confirmationToken));
            }

            string uri = Api.MarketingApi.ConfirmNewsletterSubscription(_remoteServiceBaseUrl);

            var dto = new NewsletterSubscriptionConfirmationCommand(CultureInfo.CurrentCulture, email, confirmationToken);

            var bodyContent = new StringContent(JsonConvert.SerializeObject(dto), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, bodyContent);

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception("Error in newsletter subscription process (confirmation), try later.");
            }

            response.EnsureSuccessStatusCode();
        }
    }

    public class SubscribeToNewsletterCommand : BaseMarketingCommand
    {
        public string EmailAddress { get; set; }

        public SubscribeToNewsletterCommand(CultureInfo culture, string email)
            : base(culture)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            this.EmailAddress = email;
        }
    }

    public class NewsletterSubscriptionConfirmationCommand : BaseMarketingCommand
    {
        public string EmailAddress { get; set; }
        public string ConfirmationToken { get; set; }

        public NewsletterSubscriptionConfirmationCommand(CultureInfo culture, string email, string token)
            : base(culture)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            this.EmailAddress = email;
            this.ConfirmationToken = token;
        }
    }

    public abstract class BaseMarketingCommand
    {
        public string Language { get; }

        protected BaseMarketingCommand(CultureInfo culture)
        {
            this.Language = culture.Name.StartsWith("fr") ? "fr" : "en";
        }
    }
}