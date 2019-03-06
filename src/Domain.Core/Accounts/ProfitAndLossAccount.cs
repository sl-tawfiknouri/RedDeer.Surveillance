using Domain.Core.Financial;

namespace Domain.Core.Trading
{
    public class ProfitAndLossStatement
    {
        public ProfitAndLossStatement(Currency denomination, Money revenue, Money costs)
        {
            Denomination = denomination;
            Revenue = revenue;
            Costs = costs;
        }

        public Currency Denomination { get; }
        public Money Revenue { get; }
        public Money Costs { get; }

        public Money Profits()
        {
            return Revenue - Costs;
        }
    }
}
