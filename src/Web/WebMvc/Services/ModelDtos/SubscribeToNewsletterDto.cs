namespace OpenCodeCamp.WebMvc.Services.ModelDtos
{
    using System.ComponentModel.DataAnnotations;

    public class SubscribeToNewsletterDto
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}