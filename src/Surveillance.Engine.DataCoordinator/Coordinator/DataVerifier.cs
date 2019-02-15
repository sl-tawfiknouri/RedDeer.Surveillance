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
            await _ordersRepository.LivenCompletedOrderSets();
            _logger.LogInformation($"DataVerifier completed scanning");
        }
    }
}
