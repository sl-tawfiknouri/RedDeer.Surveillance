using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Stub;
using Surveillance.Rules.Marking_The_Close.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Marking_The_Close
{
    public class MarkingTheCloseRule : BaseUniverseRule, IMarkingTheCloseRule
    {
        private readonly ILogger _logger;

        public MarkingTheCloseRule(
            MarkingTheCloseParameters parameters,
            ILogger<MarkingTheCloseRule> logger)
            : base(
                parameters?.Window ?? TimeSpan.FromMinutes(30),
                Domain.Scheduling.Rules.MarkingTheClose,
                "V1.0",
                "Marking The Close",
                logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            if (history == null)
            {
                return;
            }
        }

        protected override void Genesis()
        {
            _logger.LogDebug($"Genesis occurred in the Marking The Close Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Market Open ({exchange?.MarketId}) occurred in Marking The Close Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Market Close ({exchange?.MarketId}) occurred in Marking The Close Rule at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogDebug($"Eschaton occured in Marking The Close Rule");
        }
    }
}
