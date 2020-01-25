namespace OpenCodeCamp.Services.OutgoingCommunications.Infrastructure.Idempotency
{
    using OpenCodeCamp.Services.OutgoingCommunications.Domain.Exceptions;
    using System;
    using System.Threading.Tasks;

    public class RequestManager : IRequestManager
    {
        private readonly OutgoingCommunicationsContext _context;

        public RequestManager(OutgoingCommunicationsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.
                FindAsync<ClientRequest>(id);

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var exists = await ExistAsync(id);

            var request = exists ?
                throw new OutgoingCommunicationsDomainException($"Request with {id} already exists") :
                new ClientRequest()
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            _context.Add(request);

            await _context.SaveChangesAsync();
        }
    }
}