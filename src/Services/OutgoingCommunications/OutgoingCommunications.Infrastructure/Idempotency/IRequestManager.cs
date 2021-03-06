﻿namespace OpenCodeCamp.Services.OutgoingCommunications.Infrastructure.Idempotency
{
    using System;
    using System.Threading.Tasks;

    public interface IRequestManager
    {
        Task<bool> ExistAsync(Guid id);

        Task CreateRequestForCommandAsync<T>(Guid id);
    }
}