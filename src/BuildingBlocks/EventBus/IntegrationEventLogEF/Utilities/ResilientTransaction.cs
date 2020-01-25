﻿namespace OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF.Utilities
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using OpenCodeCamp.BuildingBlocks.EventBus.Events;
    using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF.Services;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Text;
    using System.Threading.Tasks;

    public class ResilientTransaction
    {
        private DbContext _context;
        private ResilientTransaction(DbContext context) =>
            _context = context ?? throw new ArgumentNullException(nameof(context));

        public static ResilientTransaction New(DbContext context) =>
            new ResilientTransaction(context);

        public async Task ExecuteAsync(Func<Task> action)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    await action();
                    transaction.Commit();
                }
            });
        }
    }
}