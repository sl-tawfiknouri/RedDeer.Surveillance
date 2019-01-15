using System;
using System.Threading.Tasks;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Trade
{
    public class OrderAllocationRepository
    {
        private readonly IConnectionStringFactory _connectionFactory;
        private readonly ILogger<OrderAllocationRepository> _logger;

        private const string InsertAttributionSql = @"
            INSERT INTO 
                OrdersAllocation (OrderId, Fund, Strategy, OrderFilledVolume)
                VALUES(@OrderId, @Fund, @Strategy, @OrderFilledVolume);";

        private const string GetAttributionSql = @"
            SELECT
                Id as Id,
                OrderId as OrderId,
                Fund as Fund,
                Strategy as Strategy,
                OrderFilledVolume as OrderFilledVolume
            FROM OrdersAllocation
            WHERE OrderId in (@OrderIds);";

        public OrderAllocationRepository(
            IConnectionStringFactory connectionFactory,
            ILogger<OrderAllocationRepository> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(OrderAllocation entity)
        {
            _logger.LogInformation($"OrderAllocationRepository Create method called");



            _logger.LogInformation($"OrderAllocationRepository Create method completed");
        }

        public async Task<object> Get(object entity)
        {
            _logger.LogInformation($"OrderAllocationRepository Get method called");



            _logger.LogInformation($"OrderAllocationRepository Get method completed");

            return new object();
        }

        public class OrderAllocationDto
        {
            public string Id { get; set; }
            public string OrderId { get; set; }
            public string Fund { get; set; }
            public string Strategy { get; set; }
            public string ClientAccountId { get; set; }
            public long OrderFilledVolume { get; set; }
        }
    }
}
