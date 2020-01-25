namespace OpenCodeCamp.Services.Marketing.Domain.Helpers
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Generic methods to deal with email addresses.
    /// </summary>
    internal static class EmailsHelper
    {
        /// <summary>
        /// Checks if the format of an email address is valid or not.
        /// </summary>
        /// <param name="email">email address to check.</param>
        /// <returns>Returns if the format of an email address is valid or not.</returns>
        internal static bool IsEmailFormatValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            Regex regex = new Regex(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(email))
            {
                return false;
            }

            return new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email);
        }
    }
}