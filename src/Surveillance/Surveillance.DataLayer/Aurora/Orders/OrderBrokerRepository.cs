using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core.Trading.Orders;
using Domain.Core.Trading.Orders.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;

namespace Surveillance.DataLayer.Aurora.Orders
{
    public class OrderBrokerRepository : IOrderBrokerRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<OrderBrokerRepository> _logger;

        // INSERT OR CREATE
        private const string InsertBrokerSql = @"";

        private const string GetBrokerUnEnrichedSql = @"";
        
        public OrderBrokerRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<OrderBrokerRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string InsertOrUpdateBroker(IOrderBroker brokerName)
        {
            _logger?.LogInformation($"{brokerName?.Name} about to insert or update broker");

            if (brokerName == null)
            {
                return string.Empty;
            }

            return string.Empty;
        }

        public async Task<IReadOnlyCollection<IOrderBroker>> GetUnEnrichedBrokers()
        {
            _logger?.LogInformation($"Fetching un enriched brokers");

            return await Task.FromResult(new IOrderBroker[0]);
        }
    }
}
