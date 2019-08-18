using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    /// <summary>
    /// Potential candidate to move to data import in the future
    /// Leaving it in surveillance as it's fairly incestuous with respect to
    /// knowledge of internal business requirements
    /// Once we have a web api up for surveillance we will probably have a
    /// shared data importing project for file import and api where this should live
    /// </summary>
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
            _logger.LogInformation($"Scanning");

            _logger.LogInformation($"livening completed order sets");
            await _ordersRepository.LivenCompletedOrderSets();
            _logger.LogInformation($"completed livening order sets");

            var stalenessIndicator = DateTime.UtcNow.AddHours(-1);

            _logger.LogInformation($"fetching stale orders unlivened and older then {stalenessIndicator}");
            var staleOrders = await _ordersRepository.StaleOrders(stalenessIndicator);
            var staleOrderAllocations = await _orderAllocationsRepository.GetStaleOrderAllocations(stalenessIndicator);
            _logger.LogInformation($"completed fetching stale orders unlivened and older then {stalenessIndicator}");

            if (staleOrders != null && staleOrders.Any())
            {
                _logger.LogWarning($"scan found {staleOrders.Count} orders without corresponding order allocations. About to print out their order ids and creation dates. CLIENTSERVICES");

                foreach (var order in staleOrders)
                    _logger.LogWarning($"scan found order {order.OrderId} last updated on {order.CreatedDate} which did not have any allocations. CLIENTSERVICES");
            }

            if (staleOrderAllocations != null && staleOrderAllocations.Any())
            {
                _logger.LogWarning($"scan found {staleOrderAllocations.Count} order allocations without corresponding orders. About to print out their order ids and creation dates. CLIENTSERVICES");

                foreach (var allocation in staleOrderAllocations)
                    _logger.LogWarning($"scan found order allocation for order {allocation.OrderId} last updated on {allocation.CreatedDate} which did not have any order data. CLIENTSERVICES");
            }

            _logger.LogInformation($"completed scanning");
        }
    }
}
