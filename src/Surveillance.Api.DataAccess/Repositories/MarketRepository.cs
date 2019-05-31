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
    public class MarketRepository : IMarketRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public MarketRepository(IGraphQlDbContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IEnumerable<IMarket>> GetAllDb()
        {
            using (var dbContext = _factory.Build())
            {
                var markets = await dbContext.Market.AsNoTracking().ToListAsync();

                return markets;
            }
        }

        public async Task<IEnumerable<IMarket>> Query(Func<IQueryable<IMarket>, IQueryable<IMarket>> query)
        {
            using (var dbContext = _factory.Build())
            {
                var markets = await query(dbContext.Market).AsNoTracking().ToListAsync();

                return markets;
            }
        }

        public async Task<IMarket> GetById(int id)
        {
            using (var dbContext = _factory.Build())
            {
                var market = await dbContext
                    .Market
                    .AsNoTracking()
                    .SingleOrDefaultAsync(s => s.Id == id);

                return market;
            }
        }
    }
}
