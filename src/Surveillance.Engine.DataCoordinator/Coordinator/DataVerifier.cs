using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    public class DataVerifier : IDataVerifier
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IOrderAllocationRepository _orderAllocationsRepository;
        private readonly ILogger<DataVerifier> _logger;

        public DataVerifier(
            IOrdersRepository ordersRepository,
            IOrderAllocationRepository orderAllocationsRepository,
            ILogger<DataVerifier> logger)
        {
            _ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            _orderAllocationsRepository = orderAllocationsRepository ?? throw new ArgumentNullException(nameof(orderAllocationsRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Scan()
        {
            _logger.LogInformation($"DataVerifier Scanning");

            _logger.LogInformation($"DataVerifier livening completed order sets");
            await _ordersRepository.LivenCompletedOrderSets();
            _logger.LogInformation($"DataVerifier completed livening order sets");

            var stalenessIndicator = DateTime.UtcNow.AddDays(-1);

            _logger.LogInformation($"DataVerifier fetching stale orders unlivened and older then {stalenessIndicator}");
            var staleOrders = await _ordersRepository.StaleOrders(stalenessIndicator);
            var staleOrderAllocations = await _orderAllocationsRepository.GetStaleOrderAllocations(stalenessIndicator);
            _logger.LogInformation($"DataVerifier completed fetching stale orders unlivened and older then {stalenessIndicator}");

            if (staleOrders?.Any() ?? false)
            {
                _logger.LogError($"DataVerifier scan found orders without corresponding order allocations. About to print out their order ids and creation dates. CLIENTSERVICES");

                foreach (var order in staleOrders)
                    _logger.LogError($"DataVerifier scan found order {order.OrderId} last updated on {order.CreatedDate} which did not have any allocations. CLIENTSERVICES");
            }

            if (staleOrderAllocations?.Any() ?? false)
            {
                _logger.LogError($"DataVerifier scan found order allocations without corresponding orders. About to print out their order ids and creation dates. CLIENTSERVICES");

                foreach (var allocation in staleOrderAllocations)
                    _logger.LogError($"DataVerifier scan found order allocation for order {allocation.OrderId} last updated on {allocation.CreatedDate} which did not have any order data. CLIENTSERVICES");
            }

            _logger.LogInformation($"DataVerifier completed scanning");
        }
    }
}
