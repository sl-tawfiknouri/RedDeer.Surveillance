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
    public class FinancialInstrumentRepository : IFinancialInstrumentRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public FinancialInstrumentRepository(IGraphQlDbContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        
        public async Task<IEnumerable<IFinancialInstrument>> Query(Func<IQueryable<IFinancialInstrument>, IQueryable<IFinancialInstrument>> query)
        {
            using (var dbContext = _factory.Build())
            {
                var financialInstruments =
                    await query(dbContext.FinancialInstrument)
                        .Distinct()
                        .AsNoTracking()
                        .ToListAsync();

                return financialInstruments;
            }
        }
    }
}
