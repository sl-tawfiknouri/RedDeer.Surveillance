using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Trade
{
    public class OrderAttributionRepository
    {
        private readonly IConnectionStringFactory _connectionFactory;
        private readonly ILogger<OrderAttributionRepository> _logger;

        private const string InsertAttributionSql = @"
            INSERT INTO 
                OrdersAttribution (OrderId, Fund, Strategy, OrderFilledVolume)
                VALUES(@OrderId, @Fund, @Strategy, @OrderFilledVolume);";

        private const string GetAttributionSql = @"
            SELECT
                Id as Id,
                OrderId as OrderId,
                Fund as Fund,
                Strategy as Strategy,
                OrderFilledVolume as OrderFilledVolume
            FROM OrdersAttribution
            WHERE OrderId in (@OrderIds);";

        public OrderAttributionRepository(
            IConnectionStringFactory connectionFactory,
            ILogger<OrderAttributionRepository> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(object entity)
        {
            _logger.LogInformation($"OrderAttributionRepository Create method called");

            _logger.LogInformation($"OrderAttributionRepository Create method completed");
        }

        public async Task<object> Get(object entity)
        {
            _logger.LogInformation($"OrderAttributionRepository Get method called");

            _logger.LogInformation($"OrderAttributionRepository Get method completed");

            return new object();
        }

        public class OrderAttributionDto
        {
            public string Id { get; set; }
            public string OrderId { get; set; }
            public string Fund { get; set; }
            public string Strategy { get; set; }
            public long OrderFilledVolume { get; set; }
        }
    }
}
