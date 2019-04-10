using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.DataAccess.Repositories
{
    public class RuleBreachRepository : IRuleBreachRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public RuleBreachRepository(IGraphQlDbContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IEnumerable<IRuleBreach>> GetAllDb()
        {
            using (var dbContext = _factory.Build())
            {
                var ruleBreaches =
                    await dbContext
                        .RuleBreach
                        .AsNoTracking()
                        .ToListAsync();

                return ruleBreaches;
            }
        }

        public async Task<IEnumerable<IRuleBreach>> Query(Func<IQueryable<IRuleBreach>, IQueryable<IRuleBreach>> query)
        {
            using (var dbContext = _factory.Build())
            {
                var ruleBreaches = await query(dbContext.RuleBreach).AsNoTracking().ToListAsync();

                return ruleBreaches;
            }
        }
    }
}
