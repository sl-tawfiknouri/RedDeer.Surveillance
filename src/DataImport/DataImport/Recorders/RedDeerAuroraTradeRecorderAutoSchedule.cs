using System;
using System.Collections.Generic;
using System.Linq;
using DataImport.Configuration.Interfaces;
using DataImport.MessageBusIO.Interfaces;
using DataImport.Recorders.Interfaces;
using DataImport.Services.Interfaces;
using DomainV2.Scheduling;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;

namespace DataImport.Recorders
{
    public class RedDeerAuroraTradeRecorderAutoSchedule : IRedDeerAuroraTradeRecorderAutoSchedule
    {
        private readonly IEnrichmentService _enrichmentService;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IScheduleRuleMessageSender _sender;
        private readonly IUploadConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        private readonly IDictionary<string, SchedulePair> _batchTracker;

        public RedDeerAuroraTradeRecorderAutoSchedule(
            IEnrichmentService enrichmentService,
            IOrdersRepository ordersRepository,
            IScheduleRuleMessageSender sender,
            IUploadConfiguration configuration,
            ILogger<RedDeerAuroraTradeRecorderAutoSchedule> logger)
        {
            _enrichmentService = enrichmentService ?? throw new ArgumentNullException(nameof(enrichmentService));
            _ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
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
                _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule received a null order value");
                return;
            }

            _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule passing value {value.OrderId} to trade repository");
            await _ordersRepository.Create(value);
            _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule completed passing value {value.OrderId} to trade repository");

            if (_configuration.AutoSchedule)
            {
                _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule about to schedule {value.OrderId}");
                UpdateBatch(value);
            }
            else
            {
                _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule did not have auto scheduling enabled. Returning");
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
                _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule discovered {value.OrderId} was not part of an input batch. Returning.");
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
                        TimeSeriesInitiation = value.PlacedDate.GetValueOrDefault(),
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

                if (value.PlacedDate.HasValue && value.PlacedDate.Value < schedulePair.Schedule.TimeSeriesInitiation)
                {
                    _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule discovered {value.OrderId} had an older placed date than the current time series initiation. Moving date backward to {value.PlacedDate.Value}.");

                    schedulePair.Schedule.TimeSeriesInitiation = value.PlacedDate.Value;
                }

                if (value.MostRecentDateEvent() > schedulePair.Schedule.TimeSeriesTermination)
                {
                    _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule discovered {value.OrderId} had an younger most recent event date than the current time series initiation. Moving date forward to {value.MostRecentDateEvent()}.");

                    schedulePair.Schedule.TimeSeriesTermination = value.MostRecentDateEvent();
                }
                
                if (schedulePair.Count == value.BatchSize)
                {
                    _batchTracker.Remove(value.InputBatchId);
                    _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule auto scheduling now dispatching run rule request as full batch size of {value.BatchSize} has been met.");

                    var scanTask = _enrichmentService.Scan();
                    var result = scanTask.Result;

                    _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule scanned for enrichment tasks and {result} found results");
                    _sender.Send(schedulePair.Schedule);
                    _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule batch dispatched");
                }
                else
                {
                    _logger.LogInformation($"RedDeerAuroraTradeRecorderAutoSchedule did not match batch message count {schedulePair.Count} to expected batch message total count {value.BatchSize}. Continuing to wait for further messages.");
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

            return allRulesList.Select(arl => new RuleIdentifier { Rule = arl, Ids = new string[0]}).ToList();
        }

        private class SchedulePair
        {
            public int Count { get; set; }
            public ScheduledExecution Schedule { get; set; }
        }
    }
}
