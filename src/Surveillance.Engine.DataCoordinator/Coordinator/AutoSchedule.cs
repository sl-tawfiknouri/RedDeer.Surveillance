using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    public class AutoSchedule : IAutoSchedule
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IScheduleRuleMessageSender _messageSender;
        private readonly ILogger<AutoSchedule> _logger;

        public AutoSchedule(
            IOrdersRepository ordersRepository,
            IScheduleRuleMessageSender messageSender,
            ILogger<AutoSchedule> logger)
        {
            _ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
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
                await _messageSender.Send(schedule);
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
            var allRules = Enum.GetValues(typeof(Rules));
            var allRulesList = new List<Rules>();

            foreach (var item in allRules)
            {
                var rule = (Rules)item;
                if (rule == Rules.UniverseFilter
                    || rule == Rules.CancelledOrders
                    || rule == Rules.Layering
                    || rule == Rules.MarkingTheClose
                    || rule == Rules.Spoofing
                    || rule == Rules.FrontRunning
                    || rule == Rules.PaintingTheTape
                    || rule == Rules.ImproperMatchedOrders
                    || rule == Rules.CrossAssetManipulation
                    || rule == Rules.PumpAndDump
                    || rule == Rules.TrashAndCash)
                    continue;

                allRulesList.Add(rule);
            }

            return allRulesList.Select(arl => new RuleIdentifier { Rule = arl, Ids = new string[0] }).ToList();
        }
    }
}
