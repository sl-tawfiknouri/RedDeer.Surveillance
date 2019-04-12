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
    public class SystemProcessRepository : ISystemProcessRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public SystemProcessRepository(IGraphQlDbContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IEnumerable<ISystemProcess>> GetAllDb()
        {
            using (var dbContext = _factory.Build())
            {
                var processes =
                    await dbContext
                        .SystemProcess
                        .AsNoTracking()
                        .ToListAsync();

                return processes;
            }
        }

        public async Task<ISystemProcess> GetForId(string id)
        {
            using (var dbContext = _factory.Build())
            {
                var operations =
                    await dbContext
                        .SystemProcess
                        .Where(i => i.Id == id)
                        .ToListAsync();

                return operations.FirstOrDefault();
            }
        }
    }
}
