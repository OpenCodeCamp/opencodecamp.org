namespace UnitTest.Marketing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class FakeDataProvider
    {
        internal const string valid_french_language_code = "fr";
        internal const string valid_english_language_code = "en";
        internal const string valid_email_address = "marketing@unit-tests.com";

        /// <summary>
        /// Collection of wrong formatted email addresses.
        /// </summary>
        /// <remarks>It does not contain a null or an empty value.</remarks>
        internal static string[] invalid_emails = new string[]
        {
            "@test.com",
            "test@test.",
            "test@test",
            "test@@test.com",
            "test@test..com",
            "this is not an email"
        };

        internal static string invalid_email
        {
            get
            {
                Random random = new Random();
                return invalid_emails.Skip(random.Next(0, invalid_emails.Count() - 1)).First();
            }
        }

        /// <summary>
        /// Collection of wrong formatted email addresses.
        /// </summary>
        /// <remarks>It does not contain a null or an empty value.</remarks>
        internal static IEnumerable<object[]> invalid_emails_objects
        {
            get
            {
                IList<object[]> emails = new List<object[]>();
                foreach (var invalid_email in invalid_emails)
                {
                    emails.Add(new object[] { invalid_email });
                }
                return emails;
            }
        }

        internal static string[] invalid_languages = new string[]
        {
            "f",
            "english",
            "1"
        };

        /// <summary>
        /// Collection of invalid languages.
        /// </summary>
        /// <remarks>It does not contain a null or an empty value.</remarks>
        internal static IEnumerable<object[]> invalid_languages_objects => new List<object[]>
        {
            new object[]{invalid_languages[0] },
            new object[]{invalid_languages[1]},
            new object[]{ invalid_languages [2] }
        };

        internal static string invalid_language
        {
            get
            {
                Random random = new Random();
                return invalid_languages.Skip(random.Next(0, invalid_languages.Count() - 1)).First();
            }
        }

        internal static class NewsletterSubscriptionFakeDataProvider
        {
            internal static string confirmation_token => Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}