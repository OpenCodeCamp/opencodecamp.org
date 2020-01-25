namespace OpenCodeCamp.Services.OutgoingCommunications.Infrastructure.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenCodeCamp.Services.OutgoingCommunications.Infrastructure.Idempotency;

    class ClientRequestEntityTypeConfiguration
        : IEntityTypeConfiguration<ClientRequest>
    {
        public void Configure(EntityTypeBuilder<ClientRequest> requestConfiguration)
        {
            requestConfiguration.ToTable("requests", OutgoingCommunicationsContext.DEFAULT_SCHEMA);
            requestConfiguration.HasKey(cr => cr.Id);
            requestConfiguration.Property(cr => cr.Name).IsRequired();
            requestConfiguration.Property(cr => cr.Time).IsRequired();
        }
    }
}