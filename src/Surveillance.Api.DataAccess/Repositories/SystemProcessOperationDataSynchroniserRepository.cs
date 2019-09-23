namespace Surveillance.Api.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class SystemProcessOperationDataSynchroniserRepository : ISystemProcessOperationDataSynchroniserRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public SystemProcessOperationDataSynchroniserRepository(IGraphQlDbContextFactory factory)
        {
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IEnumerable<ISystemProcessOperationDataSynchroniser>> GetAllDb()
        {
            using (var dbContext = this._factory.Build())
            {
                var dataSynchroniserRequests = await dbContext.DataSynchroniser.AsNoTracking().ToListAsync();

                return dataSynchroniserRequests;
            }
        }
    }
}