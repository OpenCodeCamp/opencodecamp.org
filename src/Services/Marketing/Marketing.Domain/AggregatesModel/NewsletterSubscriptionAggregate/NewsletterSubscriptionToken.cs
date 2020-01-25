namespace OpenCodeCamp.Services.Marketing.Domain.AggregatesModel.NewsletterSubscriptionAggregate
{
    using OpenCodeCamp.Services.Marketing.Domain.Exceptions;
    using OpenCodeCamp.Services.Marketing.Domain.Seedwork;
    using OpenCodeCamp.Services.Marketing.Domain.Events;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class NewsletterSubscriptionToken
         : Entity
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private string _token;
        public string Token => this._token;

        //private int _typeCode;
        //public int TypeCode => this._typeCode;
        //public NewsletterSubscriptionTokenTypes TokenType => (NewsletterSubscriptionTokenTypes)this._typeCode;

        private int _tokenTypeId;
        public NewsletterSubscriptionTokenType TokenType { get; private set; }
        public int TokenTypeId => this._tokenTypeId;

        public DateTime Inserted => this._inserted;
        private DateTime _inserted;

        private DateTime expiration;

        private const int _TOKEN_VALIDATION_DAYS = 5;

        private DateTime? _used;

        public NewsletterSubscriptionToken()
        {
            this._inserted = DateTime.Now;
        }

        public NewsletterSubscriptionToken(NewsletterSubscriptionTokenType tokenType)
            : this()
        {
            this._token = this.GenerateToken();
            this._tokenTypeId = tokenType.Id;
            //this.TokenType = tokenType;
            this.expiration = DateTime.Now.AddDays(_TOKEN_VALIDATION_DAYS);
        }

        private string GenerateToken() =>
            Guid.NewGuid().ToString().Replace("-", String.Empty);

        public bool IsValid() =>
            this.expiration.Date > DateTime.Now.Date && !this._used.HasValue;

        public void Use()
        {
            if (string.IsNullOrWhiteSpace(this._token))
            {
                throw new InvalidOperationException("This token cannot be use, the token is empty.");
            }

            if (this.TokenTypeId <= 0)
            {
                throw new InvalidOperationException("This token cannot be use, it needs a token type identifier.");
            }

            if (this._used.HasValue)
            {
                throw new InvalidOperationException("This token is already used.");
            }

            this._used = DateTime.Now;
        }
    }
}