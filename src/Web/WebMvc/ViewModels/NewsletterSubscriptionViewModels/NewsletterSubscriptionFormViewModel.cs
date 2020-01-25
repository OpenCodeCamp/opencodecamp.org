namespace OpenCodeCamp.WebMvc.ViewModels.NewsletterSubscriptionViewModels
{
    using System.ComponentModel.DataAnnotations;

    public sealed class NewsletterSubscriptionFormViewModel
    {
        /// <summary>
        /// Email address to subscribe.
        /// </summary>
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}