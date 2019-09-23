namespace Surveillance.Api.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class SystemProcessOperationRepository : ISystemProcessOperationRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public SystemProcessOperationRepository(IGraphQlDbContextFactory factory)
        {
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IEnumerable<ISystemProcessOperation>> GetAllDb()
        {
            using (var dbContext = this._factory.Build())
            {
                var operations = await dbContext.ProcessOperation.AsNoTracking().ToListAsync();

                return operations;
            }
        }

        public async Task<ISystemProcessOperation> GetForId(int id)
        {
            using (var dbContext = this._factory.Build())
            {
                var operations = await dbContext.ProcessOperation.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

                return operations;
            }
        }
    }
}