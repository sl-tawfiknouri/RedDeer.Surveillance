
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Scheduling;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Configuration.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.Recorders.Interfaces;

namespace Surveillance.Recorders
{
    public class RedDeerAuroraTradeRecorderAutoSchedule : IRedDeerAuroraTradeRecorderAutoSchedule
    {
        private readonly IReddeerTradeRepository _tradeRepository;
        private readonly IScheduleRuleMessageSender _sender;
        private readonly IRuleConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        private readonly IDictionary<string, SchedulePair> _batchTracker;

        public RedDeerAuroraTradeRecorderAutoSchedule(
            IReddeerTradeRepository tradeRepository,
            IScheduleRuleMessageSender sender,
            IRuleConfiguration configuration,
            ILogger<RedDeerAuroraTradeRecorderAutoSchedule> logger)
        {
            _tradeRepository = tradeRepository ?? throw new ArgumentNullException(nameof(tradeRepository));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
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
            if (value == null)
            {
                return;
            }

            await _tradeRepository.Create(value);

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

        private List<RuleIdentifier> GetAllRules()
        {
            var allRules = Enum.GetValues(typeof(Domain.Scheduling.Rules));
            var allRulesList = new List<Domain.Scheduling.Rules>();

            foreach (var item in allRules)
            {
                allRulesList.Add((Domain.Scheduling.Rules)item);
            }

            return allRulesList.Select(arl => new RuleIdentifier { Rule = arl, Ids = new string[0]}).ToList();
        }

        private class SchedulePair
        {
            public int Count { get; set; }
            public ScheduledExecution Schedule { get; set; }
        }
    }
}
