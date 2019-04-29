using Surveillance.Api.DataAccess.Abstractions.DbContexts;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Tests
{
    public class DbContextFactory : IGraphQlDbContextFactory
    {
        public IGraphQlDbContext Build()
        {
            return null;
        }
    }
}
