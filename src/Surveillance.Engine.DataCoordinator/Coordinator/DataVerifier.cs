namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Orders.Interfaces;
    using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;

    /// <summary>
    ///     Potential candidate to move to data import in the future
    ///     Leaving it in surveillance as it's fairly incestuous with respect to
    ///     knowledge of internal business requirements
    ///     Once we have a web application programming interface up for surveillance we will probably have a
    ///     shared data importing project for file import and where this should live
    /// </summary>
    public class DataVerifier : IDataVerifier
    {
        /// <summary>
        /// The orders repository.
        /// </summary>
        private readonly IOrdersRepository ordersRepository;

        /// <summary>
        /// The order allocations repository.
        /// </summary>
        private readonly IOrderAllocationRepository orderAllocationsRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DataVerifier> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataVerifier"/> class.
        /// </summary>
        /// <param name="ordersRepository">
        /// The orders repository.
        /// </param>
        /// <param name="orderAllocationsRepository">
        /// The order allocations repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DataVerifier(
            IOrdersRepository ordersRepository,
            IOrderAllocationRepository orderAllocationsRepository,
            ILogger<DataVerifier> logger)
        {
            this.ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            this.orderAllocationsRepository =
                orderAllocationsRepository ?? throw new ArgumentNullException(nameof(orderAllocationsRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The scan.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Scan()
        {
            this.logger.LogInformation("Scanning");

            this.logger.LogInformation("livening completed order sets");
            await this.ordersRepository.LivenCompletedOrderSets();
            this.logger.LogInformation("completed livening order sets");

            var stalenessIndicator = DateTime.UtcNow.AddHours(-1);

            this.logger.LogInformation($"fetching stale orders unlivened and older then {stalenessIndicator}");
            var staleOrders = await this.ordersRepository.StaleOrders(stalenessIndicator);
            var staleOrderAllocations = await this.orderAllocationsRepository.GetStaleOrderAllocations(stalenessIndicator);
            this.logger.LogInformation($"completed fetching stale orders unlivened and older then {stalenessIndicator}");

            if (staleOrders != null && staleOrders.Any())
            {
                this.logger.LogWarning(
                    $"scan found {staleOrders.Count} orders without corresponding order allocations. About to print out their order ids and creation dates. CLIENTSERVICES");

                foreach (var order in staleOrders)
                {
                    this.logger.LogWarning(
                        $"scan found order {order.OrderId} last updated on {order.CreatedDate} which did not have any allocations. CLIENTSERVICES");
                }
            }

            if (staleOrderAllocations != null && staleOrderAllocations.Any())
            {
                this.logger.LogWarning(
                    $"scan found {staleOrderAllocations.Count} order allocations without corresponding orders. About to print out their order ids and creation dates. CLIENTSERVICES");

                foreach (var allocation in staleOrderAllocations)
                    this.logger.LogWarning(
                        $"scan found order allocation for order {allocation.OrderId} last updated on {allocation.CreatedDate} which did not have any order data. CLIENTSERVICES");
            }

            this.logger.LogInformation("completed scanning");
        }
    }
}