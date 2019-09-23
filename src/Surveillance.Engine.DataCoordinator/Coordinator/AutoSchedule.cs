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

    public class AutoSchedule : IAutoSchedule
    {
        private readonly IActiveRulesService _activeRulesService;

        private readonly ILogger<AutoSchedule> _logger;

        private readonly IOrdersRepository _ordersRepository;

        private readonly IQueueScheduleRulePublisher _publisher;

        public AutoSchedule(
            IOrdersRepository ordersRepository,
            IQueueScheduleRulePublisher publisher,
            IActiveRulesService activeRulesService,
            ILogger<AutoSchedule> logger)
        {
            this._ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            this._publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this._activeRulesService =
                activeRulesService ?? throw new ArgumentNullException(nameof(activeRulesService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Scan()
        {
            try
            {
                this._logger?.LogInformation("about to scan for auto scheduling.");

                var orders = await this._ordersRepository.LiveUnscheduledOrders();
                var filteredOrders = orders?.Where(i => i?.PlacedDate != null)?.ToList();

                if (filteredOrders == null || !filteredOrders.Any())
                {
                    this._logger?.LogInformation("found no orders requiring scheduling. Exiting.");
                    return;
                }

                var initiationDate = filteredOrders.Min(i => i.PlacedDate);
                var terminationDate = filteredOrders.Max(i => i.MostRecentDateEvent());

                if (initiationDate == null)
                {
                    this._logger?.LogInformation("found no orders requiring scheduling with valid date sets. Exiting.");
                    return;
                }

                this._logger?.LogInformation("found orders requiring scheduling. Constructing schedule.");
                var schedule = this.BuildSchedule(initiationDate.Value, terminationDate);

                this._logger?.LogInformation("about to dispatch schedule to the queue");
                await this._publisher.Send(schedule);
                this._logger?.LogInformation("finished dispatched schedule to the queue");

                await this._ordersRepository.SetOrdersScheduled(filteredOrders);
                this._logger?.LogInformation("finished updating orders with scheduled status. Completing.");
            }
            catch (Exception e)
            {
                this._logger?.LogError($"exception {e.Message}");
            }
        }

        private ScheduledExecution BuildSchedule(DateTime initiation, DateTime termination)
        {
            if (initiation > termination) initiation = termination;

            var schedule = new ScheduledExecution
                               {
                                   Rules = this.GetAllRules(),
                                   TimeSeriesInitiation = initiation,
                                   TimeSeriesTermination = termination
                               };

            return schedule;
        }

        private List<RuleIdentifier> GetAllRules()
        {
            return this._activeRulesService.EnabledRules()
                .Select(arl => new RuleIdentifier { Rule = arl, Ids = new string[0] }).ToList();
        }
    }
}