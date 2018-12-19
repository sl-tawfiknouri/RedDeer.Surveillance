﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataImport.Configuration.Interfaces;
using DataImport.MessageBusIO.Interfaces;
using DataImport.Recorders.Interfaces;
using DomainV2.Scheduling;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;

namespace DataImport.Recorders
{
    public class RedDeerAuroraTradeRecorderAutoSchedule : IRedDeerAuroraTradeRecorderAutoSchedule
    {
        private readonly IReddeerTradeRepository _tradeRepository;
        private readonly IScheduleRuleMessageSender _sender;
        private readonly IUploadConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        private readonly IDictionary<string, SchedulePair> _batchTracker;

        public RedDeerAuroraTradeRecorderAutoSchedule(
            IReddeerTradeRepository tradeRepository,
            IScheduleRuleMessageSender sender,
            IUploadConfiguration configuration,
            ILogger<RedDeerAuroraTradeRecorderAutoSchedule> logger)
        {
            _tradeRepository = tradeRepository ?? throw new ArgumentNullException(nameof(tradeRepository));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _batchTracker = new Dictionary<string, SchedulePair>();
        }

        public void OnCompleted()
        {}

        public void OnError(Exception error)
        {
            _logger.LogError($"An exception occured in the reddeer trade recorder {error}");
        }

        public async void OnNext(Order value)
        {
            if (value == null)
            {
                return;
            }
            
            await _tradeRepository.Create(value);

            if (_configuration.AutoSchedule)
            {
                UpdateBatch(value);
            }
        }

        private void UpdateBatch(Order value)
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
                        TimeSeriesInitiation = value.OrderPlacedDate.GetValueOrDefault(),
                        TimeSeriesTermination = value.MostRecentDateEvent()
                    };

                    schedulePair = new SchedulePair
                    {
                        Count = 0,
                        Schedule = schedule
                    };

                    _batchTracker.Add(value.InputBatchId, schedulePair);
                }

                schedulePair.Count += 1;

                if (value.OrderPlacedDate.HasValue && value.OrderPlacedDate.Value < schedulePair.Schedule.TimeSeriesInitiation)
                {
                    schedulePair.Schedule.TimeSeriesInitiation = value.OrderPlacedDate.Value;
                }

                if (value.MostRecentDateEvent() > schedulePair.Schedule.TimeSeriesTermination)
                {
                    schedulePair.Schedule.TimeSeriesTermination = value.MostRecentDateEvent();
                }

                if (schedulePair.Count == value.BatchSize)
                {
                    _batchTracker.Remove(value.InputBatchId);
                    _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule auto scheduling now dispatching run rule request as full batch size of {value.BatchSize} has been met.");
                    _sender.Send(schedulePair.Schedule);
                    _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule batch dispatched");
                }
            }
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
                    || rule == Rules.Spoofing)
                    continue;

                allRulesList.Add(rule);
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
