using System;
using System.Data;
using MySql.Data.MySqlClient;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Aurora
{
    public class ConnectionStringFactory : IConnectionStringFactory
    {
        private readonly IDataLayerConfiguration _config;

        public ConnectionStringFactory(IDataLayerConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IDbConnection BuildConn()
        {
            var connection = _config.AuroraConnectionString;

            return new MySqlConnection(connection);
        }
    }
}
