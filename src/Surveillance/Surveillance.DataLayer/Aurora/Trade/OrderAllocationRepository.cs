using System;
using System.Threading.Tasks;
using Dapper;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Trade
{
    public class OrderAllocationRepository : IOrderAllocationRepository
    {
        private readonly IConnectionStringFactory _connectionFactory;
        private readonly ILogger<OrderAllocationRepository> _logger;

        private const string InsertAttributionSql = @"
            INSERT IGNORE INTO 
                OrdersAllocation (OrderId, Fund, Strategy, ClientAccountId, OrderFilledVolume)
                VALUES(@OrderId, @Fund, @Strategy, @ClientAccountId, @OrderFilledVolume);";

        private const string GetAttributionSql = @"
            SELECT
                Id as Id,
                OrderId as OrderId,
                Fund as Fund,
                Strategy as Strategy,
                ClientAccountId as ClientAccountId,
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

            if (entity == null
                || !entity.IsValid())
            {
                _logger.LogInformation($"OrderAllocationRepository Create method called with null or invalid order allocation, returning without saving");
                return;
            }

            var dbConnection = _connectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var dto = new OrderAllocationDto(entity);
                _logger.LogInformation(
                    $"OrderAllocationRepository Create method opened db connection and about to write record");
                using (var conn = dbConnection.ExecuteAsync(InsertAttributionSql, dto))
                {
                    await conn;
                    _logger.LogInformation($"OrderAllocationRepository Create method completed writing record");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"OrderAllocationRepository Create method had an exception. ", e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

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
            public OrderAllocationDto()
            { 
                // leave for dapper
            }

            public OrderAllocationDto(OrderAllocation oa)
            {
                Id = oa.Id;
                OrderId = oa.OrderId;
                Fund = oa.Fund;
                Strategy = oa.Strategy;
                ClientAccountId = oa.ClientAccountId;
                OrderFilledVolume = oa.OrderFilledVolume;
            }

            public string Id { get; set; }
            public string OrderId { get; set; }
            public string Fund { get; set; }
            public string Strategy { get; set; }
            public string ClientAccountId { get; set; }
            public long OrderFilledVolume { get; set; }
        }
    }
}
