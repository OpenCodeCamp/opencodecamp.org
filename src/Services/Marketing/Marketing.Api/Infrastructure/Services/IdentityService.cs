namespace OpenCodeCamp.Services.Marketing.Api.Infrastructure.Services
{
    using System;
    using Microsoft.AspNetCore.Http;

    public class IdentityService : IIdentityService
    {
        private IHttpContextAccessor _context;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
