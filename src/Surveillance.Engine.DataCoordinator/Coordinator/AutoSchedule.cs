using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Surveillance.Rules;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    public class AutoSchedule : IAutoSchedule
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IQueueScheduleRulePublisher _publisher;
        private readonly ILiveRulesService _liveRulesService;
        private readonly ILogger<AutoSchedule> _logger;

        public AutoSchedule(
            IOrdersRepository ordersRepository,
            IQueueScheduleRulePublisher publisher,
            ILiveRulesService liveRulesService,
            ILogger<AutoSchedule> logger)
        {
            _ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _liveRulesService = liveRulesService ?? throw new ArgumentNullException(nameof(liveRulesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Scan()
        {
            try
            {
                _logger?.LogInformation($"AutoSchedule about to scan for auto scheduling.");

                var orders = await _ordersRepository.LiveUnscheduledOrders();
                var filteredOrders = orders?.Where(i => i?.PlacedDate != null)?.ToList();

                if (filteredOrders == null
                    || !filteredOrders.Any())
                {
                    _logger?.LogInformation($"AutoSchedule found no orders requiring scheduling. Exiting.");
                    return;
                }

                var initiationDate = filteredOrders.Min(i => i.PlacedDate);
                var terminationDate = filteredOrders.Max(i => i.MostRecentDateEvent());

                if (initiationDate == null)
                {
                    _logger?.LogInformation($"AutoSchedule found no orders requiring scheduling with valid date sets. Exiting.");
                    return;
                }

                _logger?.LogInformation($"AutoSchedule found orders requiring scheduling. Constructing schedule.");
                var schedule = BuildSchedule(initiationDate.Value, terminationDate);

                _logger?.LogInformation($"AutoSchedule about to dispatch schedule to the queue");
                await _publisher.Send(schedule);
                _logger?.LogInformation($"AutoSchedule finished dispatched schedule to the queue");

                await _ordersRepository.SetOrdersScheduled(filteredOrders);
                _logger?.LogInformation($"AutoSchedule finished updating orders with scheduled status. Completing.");
            }
            catch (Exception e)
            {
                _logger?.LogError($"Autoschedule exception {e.Message}");
            }
        }

        private ScheduledExecution BuildSchedule(DateTime initiation, DateTime termination)
        {
            if (initiation > termination)
            {
                initiation = termination;
            }

            var schedule = new ScheduledExecution
            {
                Rules = GetAllRules(),
                TimeSeriesInitiation = initiation,
                TimeSeriesTermination = termination
            };

            return schedule;
        }

        private List<RuleIdentifier> GetAllRules()
        {
            return 
                _liveRulesService
                    .LiveRules()
                    .Select(arl => new RuleIdentifier { Rule = arl, Ids = new string[0] })
                    .ToList();
        }
    }
}
