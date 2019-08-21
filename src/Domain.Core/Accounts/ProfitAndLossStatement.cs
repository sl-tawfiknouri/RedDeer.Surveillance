namespace Domain.Core.Accounts
{
    using Domain.Core.Financial.Money;

    public class ProfitAndLossStatement
    {
        public ProfitAndLossStatement(Currency denomination, Money revenue, Money costs)
        {
            this.Denomination = denomination;
            this.Revenue = revenue;
            this.Costs = costs;
        }

        public Money Costs { get; }

        public Currency Denomination { get; }

        public Money Revenue { get; }

        public static ProfitAndLossStatement Empty()
        {
            return new ProfitAndLossStatement(new Currency("GBP"), new Money(0, "GBP"), new Money(0, "GBP"));
        }

        public decimal? PercentageProfits()
        {
            if (this.Revenue.Value == 0 || this.Costs.Value == 0) return null;

            return this.Revenue.Value / this.Costs.Value - 1;
        }

        public Money Profits()
        {
            return this.Revenue - this.Costs;
        }
    }
}