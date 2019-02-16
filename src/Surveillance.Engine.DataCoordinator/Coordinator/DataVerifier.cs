using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    public class DataVerifier : IDataVerifier
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly ILogger<DataVerifier> _logger;

        public DataVerifier(
            IOrdersRepository ordersRepository,
            ILogger<DataVerifier> logger)
        {
            _ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
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
            await _ordersRepository.StaleOrders(stalenessIndicator);


            _logger.LogInformation($"DataVerifier completed fetching stale orders unlivened and older then {stalenessIndicator}");



            _logger.LogInformation($"DataVerifier completed scanning");
        }
    }
}
