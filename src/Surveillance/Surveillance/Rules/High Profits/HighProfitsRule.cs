using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.High_Profits.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.High_Profits
{
    public class HighProfitsRule : BaseTradeRule, IHighProfitRule
    {
        public HighProfitsRule(
            TimeSpan windowSize,
            ILogger logger)
            : base(windowSize, Domain.Scheduling.Rules.HighProfits, "V1.0", logger)
        {
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            
        }
    }
}
