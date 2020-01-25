namespace OpenCodeCamp.Services.Marketing.Infrastructure.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using System;

    class NewsletterSubscriptionTokenEntityTypeConfiguration
     : IEntityTypeConfiguration<NewsletterSubscriptionToken>
    {
        public void Configure(EntityTypeBuilder<NewsletterSubscriptionToken> entityConfiguration)
        {
            entityConfiguration.ToTable(nameof(NewsletterSubscriptionToken).ToLower() + "s", MarketingContext.DEFAULT_SCHEMA);

            entityConfiguration.HasKey(o => o.Id);

            entityConfiguration.Ignore(b => b.DomainEvents);

            entityConfiguration.Property(o => o.Id)
               .ForSqlServerUseSequenceHiLo(nameof(NewsletterSubscriptionToken).ToLower() + "seq", MarketingContext.DEFAULT_SCHEMA);

            entityConfiguration.Property<int>("NewsletterSubscriptionId").IsRequired();

            entityConfiguration
               .Property<string>("_token")
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("Token")
               .HasMaxLength(32)
               .IsRequired();

            entityConfiguration
                .Property<int>("_tokenTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("TokenTypeId")
                .IsRequired();

            entityConfiguration.HasOne(p => p.TokenType)
                .WithMany()
                .HasForeignKey("_tokenTypeId");

            entityConfiguration.Property<DateTime>("Inserted")
                .ValueGeneratedOnAdd()
                .HasDefaultValue(DateTime.Now)
                .IsRequired();

            entityConfiguration
               .Property<DateTime>("expiration")
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("Expiration")
               .HasMaxLength(25)
               .IsRequired();

            entityConfiguration
               .Property<DateTime?>("_used")
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("Used")
               .HasMaxLength(25)
               .IsRequired(false);
        }
    }
}