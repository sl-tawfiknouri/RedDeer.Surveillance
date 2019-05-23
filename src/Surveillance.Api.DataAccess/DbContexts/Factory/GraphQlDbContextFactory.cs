using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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

        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[]
            {
                new ConsoleLoggerProvider((category, level)
                    => category == DbLoggerCategory.Database.Command.Name
                       && level == LogLevel.Information, true)
            });

        public IGraphQlDbContext Build()
        {
            var options = new DbContextOptionsBuilder<GraphQlDbContext>();
            var connectionString = _config.GetValue<string>("SurveillanceApiConnectionString");

            options
                // .UseLoggerFactory(MyLoggerFactory) // Log everything to console
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)) // Throw exception if query will be evaluated locally
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
