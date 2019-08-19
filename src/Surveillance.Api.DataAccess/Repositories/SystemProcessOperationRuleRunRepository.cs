namespace Surveillance.Api.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class SystemProcessOperationRuleRunRepository : ISystemProcessOperationRuleRunRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public SystemProcessOperationRuleRunRepository(IGraphQlDbContextFactory factory)
        {
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IEnumerable<ISystemProcessOperationRuleRun>> GetAllDb()
        {
            using (var dbContext = this._factory.Build())
            {
                var ruleRuns = await dbContext.RuleRun.Distinct().AsNoTracking().ToListAsync();

                return ruleRuns;
            }
        }

        public async Task<IEnumerable<ISystemProcessOperationRuleRun>> Query(
            Func<IQueryable<ISystemProcessOperationRuleRun>, IQueryable<ISystemProcessOperationRuleRun>> query)
        {
            using (var dbContext = this._factory.Build())
            {
                var ruleRuns = await query(dbContext.RuleRun).Distinct().AsNoTracking().ToListAsync();

                return ruleRuns;
            }
        }
    }
}