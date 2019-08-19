namespace Surveillance.Api.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class SystemProcessOperationUploadFileRepository : ISystemProcessOperationUploadFileRepository
    {
        private readonly IGraphQlDbContextFactory _graphQlContextFactory;

        public SystemProcessOperationUploadFileRepository(IGraphQlDbContextFactory graphQlContextFactory)
        {
            this._graphQlContextFactory =
                graphQlContextFactory ?? throw new ArgumentNullException(nameof(graphQlContextFactory));
        }

        public async Task<IEnumerable<ISystemProcessOperationUploadFile>> GetAllDb()
        {
            using (var dbContext = this._graphQlContextFactory.Build())
            {
                var funds = await dbContext.UploadFile.AsNoTracking().ToListAsync();

                return funds;
            }
        }
    }
}