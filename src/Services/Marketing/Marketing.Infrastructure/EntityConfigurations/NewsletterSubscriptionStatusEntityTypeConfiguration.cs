namespace OpenCodeCamp.Services.Marketing.Infrastructure.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;

    class NewsletterSubscriptionStatusEntityTypeConfiguration
    : IEntityTypeConfiguration<NewsletterSubscriptionStatus>
    {
        public void Configure(EntityTypeBuilder<NewsletterSubscriptionStatus> entityConfiguration)
        {
            entityConfiguration.ToTable(nameof(NewsletterSubscriptionStatus).ToLower(), MarketingContext.DEFAULT_SCHEMA);

            entityConfiguration.HasKey(o => o.Id);

            entityConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            entityConfiguration.Property(o => o.Name)
                .HasMaxLength(40)
                .IsRequired();
        }
    }
}