namespace Surveillance.Api.DataAccess.DbContexts.Factory
{
    using System;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

    using Surveillance.Api.DataAccess.Abstractions.DbContexts;
    using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;

    public class GraphQlDbContextFactory : IGraphQlDbContextFactory
    {
        private readonly IConfiguration _config;

        private readonly ILoggerFactory _loggerFactory;

        public GraphQlDbContextFactory(IConfiguration config, ILoggerFactory loggerFactory)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._loggerFactory = loggerFactory;
        }

        public IGraphQlDbContext Build()
        {
            var options = new DbContextOptionsBuilder<GraphQlDbContext>();
            var connectionString = this._config.GetValue<string>("SurveillanceApiConnectionString");

            if (this._loggerFactory != null) options.UseLoggerFactory(this._loggerFactory);

            options
                .ConfigureWarnings(
                    warnings => warnings.Throw(
                        RelationalEventId
                            .QueryClientEvaluationWarning)) // Throw exception if query will be evaluated locally
                .UseMySql(
                    connectionString,
                    builder => builder.CharSetBehavior(CharSetBehavior.AppendToAllColumns)
                        .UnicodeCharSet(CharSet.Utf8mb4));

            return new GraphQlDbContext(options.Options);
        }
    }
}