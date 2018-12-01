using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Finance;
using Microsoft.Extensions.Logging;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class RevenueMarkingCloseCalculator : RevenueCalculator
    {
        public RevenueMarkingCloseCalculator(ILogger<RevenueCalculator> logger)
            : base(logger)
        { }

        protected override CurrencyAmount? SecurityTickToPrice(SecurityTick tick)
        {
            if (tick == null)
            {
                return null;
            }

            return tick.IntradayPrices.Close ?? tick.Spread.Price;
        }
    }
}
