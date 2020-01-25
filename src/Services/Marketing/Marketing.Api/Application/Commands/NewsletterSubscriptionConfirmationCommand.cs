namespace OpenCodeCamp.Services.Marketing.Api.Application.Commands
{
    using System;
    using MediatR;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Collections;
    using System.Linq;

    // DDD and CQRS patterns comment: Note that it is recommended to implement immutable Commands
    // In this case, its immutability is achieved by having all the setters as private
    // plus only being able to update the data just once, when creating the object through its constructor.
    // References on Immutable Commands:  
    // http://cqrs.nu/Faq
    // https://docs.spine3.org/motivation/immutability.html 
    // http://blog.gauffin.org/2012/06/griffin-container-introducing-command-support/
    // https://msdn.microsoft.com/en-us/library/bb383979.aspx

    [DataContract]
    public class NewsletterSubscriptionConfirmationCommand
        : IRequest<bool>
    {
        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public string ConfirmationToken { get; set; }

        [DataMember]
        public string Language { get; set; }

        public NewsletterSubscriptionConfirmationCommand()
        { }

        public NewsletterSubscriptionConfirmationCommand(string language, string email, string token)
            : this()
        {
            this.Language = language;
            this.EmailAddress = email;
            this.ConfirmationToken = token;
        }
    }
}