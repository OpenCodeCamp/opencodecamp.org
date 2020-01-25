namespace UnitTest.Marketing.Domain
{
    using System;
    using Xunit;
    using OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate;
    using UnitTest.Marketing;
    using System.Collections.Generic;

    public class NewsletterSubscriptionAggregateTest
    {
        #region Members

        /// <summary>
        /// Collection of invalid emails addresses formatted to be consumed in a MemberData attribute.
        /// </summary>
        public static IEnumerable<object[]> InvalidEmails => FakeDataProvider.invalid_emails_objects;

        /// <summary>
        /// Collection of invalid languages formatted to be consumed in a MemberData attribute.
        /// </summary>
        public static IEnumerable<object[]> InvalidLanguages => FakeDataProvider.invalid_languages_objects;

        #endregion Members

        public NewsletterSubscriptionAggregateTest()
        { }

        [Fact]
        public void Create_newsletter_subscription_item_success()
        {
            // Arrange
            string valid_lang = FakeDataProvider.valid_english_language_code;
            string valid_email = FakeDataProvider.valid_email_address;

            // Act
            var newsletterSubscription = new NewsletterSubscription(valid_lang, valid_email);

            // Assert
            Assert.NotNull(newsletterSubscription);
            Assert.Equal(valid_email, newsletterSubscription.EmailAddress);
            Assert.NotNull(newsletterSubscription.Tokens);
            Assert.NotEqual<int>(0, newsletterSubscription.Tokens.Count);
            Assert.Equal<int>(1, newsletterSubscription.DomainEvents.Count);
        }

        [Fact]
        public void Create_newsletter_subscription_null_lang_fail()
        {
            // Arrange
            string invalid_lang = string.Empty;
            string valid_email = FakeDataProvider.valid_email_address;

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => new NewsletterSubscription(invalid_lang, valid_email));
        }

        [Fact]
        public void Create_newsletter_subscription_empty_lang_fail()
        {
            // Arrange
            string invalid_lang = string.Empty;
            string valid_email = FakeDataProvider.valid_email_address;

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => new NewsletterSubscription(invalid_lang, valid_email));
        }

        [Theory]
        [MemberData(nameof(InvalidLanguages))]
        public void Create_newsletter_subscription_invalid_lang_fail(string invalid_lang)
        {
            // Arrange
            string valid_email = FakeDataProvider.valid_email_address;

            // Act - Assert
            Assert.Throws<ArgumentException>(() => new NewsletterSubscription(invalid_lang, valid_email));
        }

        [Fact]
        public void Create_newsletter_subscription_null_email_fail()
        {
            // Arrange
            string valid_lang = FakeDataProvider.valid_english_language_code;

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => new NewsletterSubscription(valid_lang, null));
        }

        [Fact]
        public void Create_newsletter_subscription_empty_email_fail()
        {
            // Arrange
            string valid_lang = FakeDataProvider.valid_english_language_code;
            string invalid_email = string.Empty;

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => new NewsletterSubscription(valid_lang, invalid_email));
        }

        [Theory]
        [MemberData(nameof(InvalidEmails))]
        public void Create_newsletter_subscription_invalid_email_fail(string invalid_email)
        {
            // Arrange
            string valid_lang = FakeDataProvider.valid_english_language_code;

            // Act - Assert
            Assert.Throws<ArgumentException>(() => new NewsletterSubscription(valid_lang, invalid_email));
        }
    }
}