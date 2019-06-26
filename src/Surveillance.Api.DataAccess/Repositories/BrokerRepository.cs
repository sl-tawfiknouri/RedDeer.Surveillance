using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.DataAccess.Repositories
{
    public class BrokerRepository : IBrokerRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public BrokerRepository(IGraphQlDbContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IBroker> GetById(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            using (var dbContext = _factory.Build())
            {
                var broker = await dbContext
                    .Broker
                    .AsNoTracking()
                    .SingleOrDefaultAsync(s => s.Id == id);

                return broker;
            }
        }
    }
}
