﻿namespace OpenCodeCamp.Services.Marketing.Infrastructure.Idempotency
{
    using System;

    public class ClientRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
    }
}