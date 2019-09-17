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

        /// <summary>
        /// The query for broker entities.
        /// </summary>
        /// <param name="query">
        /// The query to filter further by.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IEnumerable<IBroker>> Query(Func<IQueryable<IBroker>, IQueryable<IBroker>> query)
        {
            using (var databaseContext = this._factory.Build())
            {
                var brokers = await query(databaseContext.Broker).AsNoTracking().ToListAsync();

                return brokers;
            }
        }
    }
}
