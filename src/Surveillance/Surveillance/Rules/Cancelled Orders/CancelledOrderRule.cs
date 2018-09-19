using System;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders
{
    /// <summary>
    /// This rule looks at unusual order cancellation from several angles.
    /// We consider rules that are cancelled by value i.e. 1 order over 1 million gbp
    /// Order cancellation ratios 
    /// </summary>
    public class CancelledOrderRule : BaseTradeRule, ICancelledOrderRule
    {
        public CancelledOrderRule(
            Domain.Scheduling.Rules rule,
            string version,
            ILogger<CancelledOrderRule> logger) 
            : base(
                TimeSpan.FromMinutes(30),
                rule,
                version,
                logger)
        {
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
               
        }
    }
}
