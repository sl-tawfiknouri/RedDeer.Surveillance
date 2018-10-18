using System;
using System.Linq;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.HighVolume
{
    public class HighVolumeRule : BaseUniverseRule, IHighVolumeRule
    {
        private readonly IHighVolumeRuleParameters _parameters;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly ILogger _logger;

        private int _alertCount = 0;
        private bool _hadMissingData = false;

        public HighVolumeRule(
            IHighVolumeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            ILogger<IHighVolumeRule> logger) 
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Scheduling.Rules.HighVolume,
                Versioner.Version(1, 0),
                "High Volume Rule",
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }
            
            var tradedVolume =
                tradeWindow
                    .Where(tr =>
                        tr.OrderStatus == OrderStatus.Fulfilled
                        || tr.OrderStatus == OrderStatus.PartialFulfilled)
                    .Sum(tr => tr.FulfilledVolume);

            var mostRecentTrade = tradeWindow.Pop();
            if (_parameters.HighVolumePercentageDaily.HasValue)
            {
                DailyVolumeCheck(mostRecentTrade);
            }

            if (_parameters.HighVolumePercentageWindow.HasValue)
            {
                WindowVolumeCheck();
            }
        }

        private void DailyVolumeCheck(TradeOrderFrame mostRecentTrade)
        {
            if (!LatestExchangeFrameBook.ContainsKey(mostRecentTrade.Market.Id))
            {
                _hadMissingData = true;
                return;
            }

            LatestExchangeFrameBook.TryGetValue(mostRecentTrade.Market.Id, out var exchangeFrame);

            if (exchangeFrame == null)
            {
                _hadMissingData = true;
                return;
            }
        }

        private void WindowVolumeCheck()
        {

        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {

        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred in the High Volume Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred in the High Volume Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred in the High Volume Rule at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured in the High Volume Rule");
            _ruleCtx.UpdateAlertEvent(_alertCount);

            if (_hadMissingData)
            {
                _ruleCtx.EndEvent().EndEventWithMissingDataError();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }

            _alertCount = 0;
        }
    }
}
