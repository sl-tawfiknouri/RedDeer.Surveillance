using System;
using System.Collections.Generic;
using Domain.Scheduling;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Configuration.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Recorders.Interfaces;
using Surveillance.Recorders.Projectors.Interfaces;

namespace Surveillance.Recorders
{
    public class RedDeerTradeRecorderAutoSchedule : IRedDeerTradeRecorderAutoSchedule
    {
        private readonly IScheduleRuleMessageSender _sender;
        private readonly IRedDeerTradeFormatRepository _repository;
        private readonly IReddeerTradeFormatProjector _projector;
        private readonly IRuleConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        private readonly IDictionary<string, SchedulePair> _batchTracker;

        public RedDeerTradeRecorderAutoSchedule(
            IScheduleRuleMessageSender sender,
            IRedDeerTradeFormatRepository repository,
            IReddeerTradeFormatProjector projector,
            IRuleConfiguration configuration,
            ILogger<RedDeerTradeRecorder> logger)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _projector = projector ?? throw new ArgumentNullException(nameof(projector));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _batchTracker = new Dictionary<string, SchedulePair>();            
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"An exception occured in the reddeer trade recorder {error}");
        }

        public async void OnNext(TradeOrderFrame value)
        {
            // project trade order frame into a ES document
            var projectedFrame = _projector.Project(value);

            if (projectedFrame == null)
            {
                return;
            }

            await _repository.Save(projectedFrame);

            if (_configuration.AutoScheduleRules.GetValueOrDefault(false))
            {
                UpdateBatch(value);
            }
        }

        private void UpdateBatch(TradeOrderFrame value)
        {
            if (value == null)
            {
                return;
            }

            if (!value.IsInputBatch)
            {
                return;
            }

            lock (_lock)
            {
                _batchTracker.TryGetValue(value.InputBatchId, out var schedulePair);

                if (schedulePair == null)
                {
                    var schedule = new ScheduledExecution
                    {
                        Rules = GetAllRules(),
                        TimeSeriesInitiation = value.StatusChangedOn,
                        TimeSeriesTermination = value.StatusChangedOn
                    };

                    schedulePair = new SchedulePair
                    {
                        Count = 0,
                        Schedule = schedule
                    };

                    _batchTracker.Add(value.InputBatchId, schedulePair);
                }

                schedulePair.Count += 1;

                if (value.StatusChangedOn < schedulePair.Schedule.TimeSeriesInitiation)
                {
                    schedulePair.Schedule.TimeSeriesInitiation = value.StatusChangedOn;
                }

                if (value.StatusChangedOn > schedulePair.Schedule.TimeSeriesTermination)
                {
                    schedulePair.Schedule.TimeSeriesTermination = value.StatusChangedOn;
                }

                if (schedulePair.Count == value.BatchSize)
                {
                    _batchTracker.Remove(value.InputBatchId);
                    _sender.Send(schedulePair.Schedule);
                }
            }
        }

        private List<Domain.Scheduling.Rules> GetAllRules()
        {
            var allRules = Enum.GetValues(typeof(Domain.Scheduling.Rules));
            var allRulesList = new List<Domain.Scheduling.Rules>();

            foreach (var item in allRules)
            {
                allRulesList.Add((Domain.Scheduling.Rules)item);
            }

            return allRulesList;
        }

        private class SchedulePair
        {
            public int Count { get; set; }
            public ScheduledExecution Schedule { get; set; }
        }
    }
}
