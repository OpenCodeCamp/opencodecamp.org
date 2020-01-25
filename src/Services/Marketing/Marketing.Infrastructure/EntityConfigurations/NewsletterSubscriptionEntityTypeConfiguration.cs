namespace OpenCodeCamp.Services.Marketing.Infrastructure.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using System;

    class NewsletterSubscriptionEntityTypeConfiguration
        : IEntityTypeConfiguration<NewsletterSubscription>
    {
        public void Configure(EntityTypeBuilder<NewsletterSubscription> entityConfiguration)
        {
            entityConfiguration.ToTable(nameof(NewsletterSubscription).ToLower() + "s", MarketingContext.DEFAULT_SCHEMA);

            entityConfiguration.HasKey(o => o.Id);

            entityConfiguration.Ignore(b => b.DomainEvents);

            entityConfiguration.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo(nameof(NewsletterSubscription).ToLower() + "seq", MarketingContext.DEFAULT_SCHEMA);

            entityConfiguration
                .Property<string>("EmailAddress")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("EmailAddress")
                .HasMaxLength(80)
                .IsRequired();

            entityConfiguration
                .Property<int>("statusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("StatusId")
                .IsRequired();

            entityConfiguration
                .Property<string>("language")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Language")
                .HasMaxLength(5)
                .IsRequired(true);

            entityConfiguration
                .Property<DateTime>("inserted")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Inserted")
                .ValueGeneratedOnAdd()
                .HasDefaultValue(DateTime.Now)
               .HasMaxLength(25)
                .IsRequired();

            entityConfiguration
                .Property<DateTime>("lastUpdated")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("LastUpdated")
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValue(DateTime.Now)
               .HasMaxLength(25)
                .IsRequired();

            entityConfiguration
                .Property<DateTime?>("confirmed")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Confirmed")
               .HasMaxLength(25)
                .IsRequired(false);

            entityConfiguration
                .Property<DateTime?>("cancelled")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Cancelled")
               .HasMaxLength(25)
                .IsRequired(false);

            entityConfiguration.HasOne(newsletterSubscription => newsletterSubscription.Status)
                .WithMany()
                .HasForeignKey("statusId");

            entityConfiguration.HasMany(newsletterSubscription => newsletterSubscription.Tokens)
                .WithOne()
                .HasForeignKey("NewsletterSubscriptionId")
                .OnDelete(DeleteBehavior.Cascade);


            var tokensNavigation = entityConfiguration.Metadata.FindNavigation(nameof(NewsletterSubscription.Tokens));
            // DDD Patterns comment:
            //Set as field (New since EF 1.1) to access the Tokens collection property through its field
            tokensNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}