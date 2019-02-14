using System;
using System.Data;
using MySql.Data.MySqlClient;
using Surveillance.Auditing.DataLayer.Interfaces;

namespace Surveillance.Auditing.DataLayer
{
    public class ConnectionStringFactory : IConnectionStringFactory
    {
        private readonly ISystemDataLayerConfig _config;

        public ConnectionStringFactory(ISystemDataLayerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IDbConnection BuildConn()
        {
            var connection = _config.SurveillanceAuroraConnectionString;

            return new MySqlConnection(connection);
        }
    }
}
