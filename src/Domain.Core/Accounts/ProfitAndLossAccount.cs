using Domain.Core.Financial;

namespace Domain.Core.Trading
{
    public class ProfitAndLossStatement
    {
        public ProfitAndLossStatement(Money revenue, Money costs)
        {
            Revenue = revenue;
            Costs = costs;
        }

        public Money Revenue { get; }
        public Money Costs { get; }

        public Money Profits()
        {
            return Revenue - Costs;
        }
    }
}
