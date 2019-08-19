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

    public class FinancialInstrumentRepository : IFinancialInstrumentRepository
    {
        private readonly IGraphQlDbContextFactory _factory;

        public FinancialInstrumentRepository(IGraphQlDbContextFactory factory)
        {
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<IFinancialInstrument> GetById(int id)
        {
            using (var dbContext = this._factory.Build())
            {
                var financialInstrument = await dbContext
                                              .FinancialInstrument.AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);

                return financialInstrument;
            }
        }

        public async Task<IEnumerable<IFinancialInstrument>> Query(
            Func<IQueryable<IFinancialInstrument>, IQueryable<IFinancialInstrument>> query)
        {
            using (var dbContext = this._factory.Build())
            {
                var financialInstruments =
                    await query(dbContext.FinancialInstrument).Distinct().AsNoTracking().ToListAsync();

                return financialInstruments;
            }
        }
    }
}