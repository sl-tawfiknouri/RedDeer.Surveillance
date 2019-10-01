namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Orders.Interfaces;
    using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
    using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

    /// <summary>
    /// The auto schedule.
    /// </summary>
    public class AutoSchedule : IAutoSchedule
    {
        /// <summary>
        /// The active rules service.
        /// </summary>
        private readonly IActiveRulesService activeRulesService;

        /// <summary>
        /// The orders repository.
        /// </summary>
        private readonly IOrdersRepository ordersRepository;

        /// <summary>
        /// The publisher.
        /// </summary>
        private readonly IQueueScheduleRulePublisher publisher;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AutoSchedule> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSchedule"/> class.
        /// </summary>
        /// <param name="ordersRepository">
        /// The orders repository.
        /// </param>
        /// <param name="publisher">
        /// The publisher.
        /// </param>
        /// <param name="activeRulesService">
        /// The active rules service.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AutoSchedule(
            IOrdersRepository ordersRepository,
            IQueueScheduleRulePublisher publisher,
            IActiveRulesService activeRulesService,
            ILogger<AutoSchedule> logger)
        {
            this.ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this.activeRulesService =
                activeRulesService ?? throw new ArgumentNullException(nameof(activeRulesService));
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
            try
            {
                this.logger?.LogInformation("about to scan for auto scheduling.");

                var orders = await this.ordersRepository.LiveUnscheduledOrders();
                var filteredOrders = orders?.Where(i => i?.PlacedDate != null)?.ToList();

                if (filteredOrders == null || !filteredOrders.Any())
                {
                    this.logger?.LogInformation("found no orders requiring scheduling. Exiting.");
                    return;
                }

                var initiationDate = filteredOrders.Min(i => i.PlacedDate);
                var terminationDate = filteredOrders.Max(i => i.MostRecentDateEvent());

                if (initiationDate == null)
                {
                    this.logger?.LogInformation("found no orders requiring scheduling with valid date sets. Exiting.");
                    return;
                }

                this.logger?.LogInformation("found orders requiring scheduling. Constructing schedule.");
                var schedule = this.BuildSchedule(initiationDate.Value, terminationDate);

                this.logger?.LogInformation("about to dispatch schedule to the queue");
                await this.publisher.Send(schedule);
                this.logger?.LogInformation("finished dispatched schedule to the queue");

                await this.ordersRepository.SetOrdersScheduled(filteredOrders);
                this.logger?.LogInformation("finished updating orders with scheduled status. Completing.");
            }
            catch (Exception e)
            {
                this.logger?.LogError($"exception {e.Message}");
            }
        }

        /// <summary>
        /// The build schedule.
        /// </summary>
        /// <param name="initiation">
        /// The initiation.
        /// </param>
        /// <param name="termination">
        /// The termination.
        /// </param>
        /// <returns>
        /// The <see cref="ScheduledExecution"/>.
        /// </returns>
        private ScheduledExecution BuildSchedule(DateTime initiation, DateTime termination)
        {
            if (initiation > termination)
            {
                initiation = termination;
            }

            var schedule = new ScheduledExecution
               {
                   Rules = this.GetAllRules(),
                   TimeSeriesInitiation = initiation,
                   TimeSeriesTermination = termination
               };

            return schedule;
        }

        /// <summary>
        /// The get all rules.
        /// </summary>
        /// <returns>
        /// The <see cref="RuleIdentifier"/>.
        /// </returns>
        private List<RuleIdentifier> GetAllRules()
        {
            return 
                this
                    .activeRulesService
                    .EnabledRules()
                    .Select(_ => new RuleIdentifier { Rule = _, Ids = new string[0] })
                    .ToList();
        }
    }
}