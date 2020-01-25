namespace OpenCodeCamp.Services.OutgoingCommunications.Api.Infrastructure.AutofacModules
{
    using Autofac;
    using OpenCodeCamp.OutgoingCommunications.Emails;
    using OpenCodeCamp.Services.OutgoingCommunications.Api.Infrastructure.Configuration;
    using OpenCodeCamp.Services.OutgoingCommunications.Api.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ApplicationModule
        : Autofac.Module
    {
        public string QueriesConnectionString { get; }
        private readonly MailSettings mailSettings;
        private readonly string emailsTemplatesDirectoryBasePath;
        private readonly string imagesBaseUrl;

        public ApplicationModule(string qconstr, MailSettings mailSettings, string emailsTemplatesDirectoryBasePath, string imagesBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(qconstr))
            {
                throw new ArgumentNullException(nameof(qconstr));
            }

            if (mailSettings == null)
            {
                throw new ArgumentNullException(nameof(mailSettings));
            }

            if (mailSettings.SMTP == null)
            {
                throw new ArgumentNullException(nameof(mailSettings.SMTP));
            }

            if (string.IsNullOrWhiteSpace(emailsTemplatesDirectoryBasePath))
            {
                throw new ArgumentNullException(nameof(emailsTemplatesDirectoryBasePath));
            }

            if (string.IsNullOrWhiteSpace(imagesBaseUrl))
            {
                throw new ArgumentNullException(nameof(imagesBaseUrl));
            }

            this.QueriesConnectionString = qconstr;
            this.mailSettings = mailSettings;
            this.emailsTemplatesDirectoryBasePath = emailsTemplatesDirectoryBasePath;
            this.imagesBaseUrl = imagesBaseUrl;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new EmailBuilderService(this.emailsTemplatesDirectoryBasePath, this.imagesBaseUrl))
                .As<IEmailBuilderService>()
                .InstancePerLifetimeScope();

            builder.Register(c => new EmailSender(this.mailSettings))
                .As<IEmailSender>()
                .InstancePerLifetimeScope();
        }
    }
}