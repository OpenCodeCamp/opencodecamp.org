namespace OpenCodeCamp.WebMvc.Infrastructure
{
    public static class Api
    {
        public static class MarketingApi
        {
            public static string CreateNewsletterSubscription(string baseUri)
            {
                return $"{baseUri}/newslettersubscriptions/create";
            }

            public static string ConfirmNewsletterSubscription(string baseUri)
            {
                return $"{baseUri}/newslettersubscriptions/confirm";
            }
        }
    }
}