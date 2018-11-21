using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Finance;
using Domain.Trades.Orders;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class RevenueCalculator
    {
        public async Task<CurrencyAmount?> CalculateRevenueOfPosition(IList<TradeOrderFrame> activeFulfilledTradeOrders)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                return new CurrencyAmount(0, string.Empty);
            }

            return null;
        }
    }
}
