namespace Surveillance.Auditing.DataLayer
{
    using System;
    using System.Data;

    using MySql.Data.MySqlClient;

    using Surveillance.Auditing.DataLayer.Interfaces;

    public class ConnectionStringFactory : IConnectionStringFactory
    {
        private readonly ISystemDataLayerConfig _config;

        public ConnectionStringFactory(ISystemDataLayerConfig config)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IDbConnection BuildConn()
        {
            var connection = this._config.SurveillanceAuroraConnectionString;

            return new MySqlConnection(connection);
        }

        public string OverrideMigrationsFolder()
        {
            return _config.OverrideMigrationsFolder;
        }
    }
}