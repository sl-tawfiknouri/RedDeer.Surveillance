﻿namespace Surveillance.Api.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class SystemProcessOperationDistributeRuleRepository : ISystemProcessOperationDistributeRuleRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public SystemProcessOperationDistributeRuleRepository(IGraphQlDbContextFactory factory)
        {
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IEnumerable<ISystemProcessOperationDistributeRule>> GetAllDb()
        {
            using (var dbContext = this._factory.Build())
            {
                var distributedRules = await dbContext.DistributeRule.AsNoTracking().ToListAsync();

                return distributedRules;
            }
        }
    }
}