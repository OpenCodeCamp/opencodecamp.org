namespace OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate
{
    using OpenCodeCamp.Services.Marketing.Domain.Exceptions;
    using OpenCodeCamp.Services.Marketing.Domain.Seedwork;
    using OpenCodeCamp.Services.Marketing.Domain.Events;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using OpenCodeCamp.Services.Marketing.Domain.Helpers;

    public class NewsletterSubscription
        : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private string emailAddress;
        public string EmailAddress => this.emailAddress;

        private DateTime inserted;

        //public string ConfirmationToken => this.confirmationToken;
        //private string confirmationToken;

        //private DateTime? confirmationEmailSent;
        private DateTime? confirmed;
        //private string cancellationToken;
        private DateTime? cancelled;

        public NewsletterSubscriptionStatus Status { get; set; }
        private int statusId;
        //public int NewsletterSubscriptionStatusId { get; private set; }


        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method OrderAggrergateRoot.AddOrderItem() which includes behaviour.
        private readonly List<NewsletterSubscriptionToken> _tokens;
        public IReadOnlyCollection<NewsletterSubscriptionToken> Tokens => this._tokens;


        private DateTime lastUpdated;

        public string Language => this.language;
        private string language;

        protected NewsletterSubscription()
        {
            this.inserted = DateTime.Now;
            this.lastUpdated = DateTime.Now;
            //this.confirmationToken = this.GenerateConfirmationToken();
            this._tokens = new List<NewsletterSubscriptionToken>();
            this.AddConfirmationtoken();
        }

        public NewsletterSubscription(string language, string email)
            : this()
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (!EmailsHelper.IsEmailFormatValid(email))
            {
                throw new ArgumentException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentNullException(nameof(language));
            }

            if (language.ToLower().Trim() != "fr" && language.ToLower().Trim() != "en")
            {
                throw new ArgumentException(nameof(language));
            }

            this.language = language.ToLower().Trim();
            this.emailAddress = email.ToLower().Trim();
            this.statusId = NewsletterSubscriptionStatus.Submitted.Id;
            //this.NewsletterSubscriptionStatusId = NewsletterSubscriptionStatus.Submitted.Id;
            //this.NewsletterSubscriptionStatus = NewsletterSubscriptionStatus.Submitted;

            // Add the NewsletterSubscriptionSubmitted to the domain events collection 
            // to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
            AddNewsletterSubscriptionSubmittedDomainEvent(email);
        }

        private void SetConfirmedStatus()
        {
            //if (this.Status.Id != NewsletterSubscriptionStatus.Submitted.Id)
            //{
            //    StatusChangeException(NewsletterSubscriptionStatus.Confirmed);
            //}
            if (this.statusId != NewsletterSubscriptionStatus.Submitted.Id)
            {
                StatusChangeException(NewsletterSubscriptionStatus.Confirmed);
            }

            AddDomainEvent(new NewsletterSubscriptionStatusChangedToConfirmedDomainEvent(Id));
            this.statusId = NewsletterSubscriptionStatus.Confirmed.Id;
            //this.NewsletterSubscriptionStatusId = NewsletterSubscriptionStatus.Confirmed.Id;

            this.confirmed = DateTime.Now;
            this.lastUpdated = DateTime.Now;
        }

        public void SetCancelledStatus()
        {
            //if (this.Status.Id != NewsletterSubscriptionStatus.AwaitingCancellationValidation.Id)
            //{
            //    StatusChangeException(NewsletterSubscriptionStatus.Cancelled);
            //}
            if (this.statusId != NewsletterSubscriptionStatus.AwaitingCancellationValidation.Id)
            {
                StatusChangeException(NewsletterSubscriptionStatus.Cancelled);
            }

            AddDomainEvent(new NewsletterSubscriptionStatusChangedToCancelledDomainEvent(Id));
            this.statusId = NewsletterSubscriptionStatus.Cancelled.Id;
            //this.NewsletterSubscriptionStatusId = NewsletterSubscriptionStatus.Cancelled.Id;
            this.cancelled = DateTime.Now;
            this.lastUpdated = DateTime.Now;
        }

        private void AddNewsletterSubscriptionSubmittedDomainEvent(string email)
        {
            string confirmationToken = this.GetConfirmationToken();

            var domainEvent = new NewsletterSubscriptionSubmittedDomainEvent(this, this.language, email, confirmationToken);

            this.AddDomainEvent(domainEvent);
        }

        public string GetConfirmationToken()
        {
            NewsletterSubscriptionToken confirmationToken = this._tokens.
                  Where(x => x.TokenTypeId == NewsletterSubscriptionTokenType.Confirmation.Id).
                  OrderByDescending(x => x.Inserted).
                  First();

            if (!confirmationToken.IsValid())
            {
                throw new MarketingDomainException("No confirmation token found");
            }

            return confirmationToken.Token;
        }

        private void StatusChangeException(NewsletterSubscriptionStatus statusToChange)
        {
            string errorMessage;
            if (this.Status != null)
            {
                errorMessage = $"Is not possible to change the order status from {Status.Name} to {statusToChange.Name}.";
            }
            else
            {
                errorMessage = $"Is not possible to change the order status to {statusToChange.Name}.";
            }

            throw new MarketingDomainException(errorMessage);
        }

        public void AddConfirmationtoken()
        {
            var token = new NewsletterSubscriptionToken(NewsletterSubscriptionTokenType.Confirmation);
            this._tokens.Add(token);
        }

        /// <summary>
        /// Confirm the newsletter subscription.
        /// </summary>
        /// <param name="token">Confirmation token.</param>
        public void Confirm(string token)
        {
            // Check the provided token.
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            // Check if the subscription's status allows the confirmation.
            //if (this.Status.Id != NewsletterSubscriptionStatus.Submitted.Id)
            //{
            //    StatusChangeException(NewsletterSubscriptionStatus.Confirmed);
            //}
            if (this.statusId != NewsletterSubscriptionStatus.Submitted.Id)
            {
                StatusChangeException(NewsletterSubscriptionStatus.Confirmed);
            }

            // Check if the provided token matches with the subscription's one.
            //if (this.ConfirmationToken != token)
            //{
            //    throw new MarketingDomainException("It's not possible to confirm this subscription, the provided token is invalid");
            //}

            // Try to get a valid token.
            if (this._tokens == null)
            {
                throw new MarketingDomainException("It's not possible to confirm this subscription, no confirmation otken available");
            }

            NewsletterSubscriptionToken lastConfirmationToken = this._tokens.
                Where(x => x.TokenTypeId == NewsletterSubscriptionTokenType.Confirmation.Id).
                OrderByDescending(x => x.Inserted).
                FirstOrDefault();

            if (lastConfirmationToken == null)
            {
                throw new MarketingDomainException("It's not possible to confirm this subscription, no confirmation token available");
            }

            if (!lastConfirmationToken.IsValid())
            {
                throw new MarketingDomainException("It's not possible to confirm this subscription, no confirmation token available");
            }

            // Use the token.
            lastConfirmationToken.Use();

            // Change the status to 'confirmed'.
            this.SetConfirmedStatus();

            // Throw a domain event.
            this.AddNewsletterSubscriptionConfirmedDomainEvent();
        }

        private void AddNewsletterSubscriptionConfirmedDomainEvent()
        {
            var domainEvent = new NewsletterSubscriptionConfirmedDomainEvent(this, this.language, this.emailAddress);

            this.AddDomainEvent(domainEvent);
        }
    }
}