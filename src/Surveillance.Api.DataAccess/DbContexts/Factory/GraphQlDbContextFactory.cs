using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Surveillance.Api.DataAccess.Abstractions.DbContexts;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
namespace Surveillance.Api.DataAccess.DbContexts.Factory
{
    public class GraphQlDbContextFactory : IGraphQlDbContextFactory
    {
        private readonly IConfiguration _config;

        public GraphQlDbContextFactory(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IGraphQlDbContext Build()
        {
            var options = new DbContextOptionsBuilder<GraphQlDbContext>();
            var connectionString = _config.GetValue<string>("SurveillanceApiConnectionString");
            
            options
                .UseMySql(
                    connectionString,
                    builder => 
                        builder
                            .CharSetBehavior(CharSetBehavior.AppendToAllColumns)
                            .UnicodeCharSet(CharSet.Utf8mb4));

            return new GraphQlDbContext(options.Options);
        }
    }
}
