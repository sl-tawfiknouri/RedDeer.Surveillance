using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Coordinator
{
    public class AutoSchedule : IAutoSchedule
    {
        private readonly IScheduleRuleMessageSender _messageSender;
        private readonly ILogger<AutoSchedule> _logger;

        public AutoSchedule(
            IScheduleRuleMessageSender messageSender,
            ILogger<AutoSchedule> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ScheduleExecution(string fileUpload)
        {
            if (string.IsNullOrWhiteSpace(fileUpload))
            {
                _logger?.LogInformation($"AutoSchedule malformed or null file upload identifier provided. Exiting.");
                return;
            }

            _logger?.LogInformation($"AutoSchedule about to schedule execution for the file upload with id {fileUpload}.");

            // fetch dates to run for from the database
            // if allocations we need to do a join
            // if orders just get the min/max dates

            // two specialist calls...to an existing repo or not?
            // I think I want to bring in 

            var schedule = BuildSchedule(DateTime.Now, DateTime.MaxValue);

            _logger?.LogInformation($"AutoSchedule about to dispatch schedule for file upload {fileUpload} to the queue");
            await _messageSender.Send(schedule);
            _logger?.LogInformation($"AutoSchedule finished dispatching schedule for file upload {fileUpload} to the queue");
        }

        private ScheduledExecution BuildSchedule(DateTime initiation, DateTime termination)
        {
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
