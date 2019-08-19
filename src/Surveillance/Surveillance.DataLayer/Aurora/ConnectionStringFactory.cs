namespace Surveillance.DataLayer.Aurora
{
    using System;
    using System.Data;

    using MySql.Data.MySqlClient;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Configuration.Interfaces;

    public class ConnectionStringFactory : IConnectionStringFactory
    {
        private readonly IDataLayerConfiguration _config;

        public ConnectionStringFactory(IDataLayerConfiguration config)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IDbConnection BuildConn()
        {
            var connection = this._config.AuroraConnectionString;

            return new MySqlConnection(connection);
        }
    }
}