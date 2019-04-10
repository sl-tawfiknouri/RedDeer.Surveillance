using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.DataAccess.Repositories
{
    public class SystemProcessOperationDataSynchroniserRepository : ISystemProcessOperationDataSynchroniserRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public SystemProcessOperationDataSynchroniserRepository(IGraphQlDbContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IEnumerable<ISystemProcessOperationDataSynchroniser>> GetAllDb()
        {
            using (var dbContext = _factory.Build())
            {
                var dataSynchroniserRequests =
                    await dbContext
                        .DataSynchroniser
                        .AsNoTracking()
                        .ToListAsync();

                return dataSynchroniserRequests;
            }
        }
    }
}
