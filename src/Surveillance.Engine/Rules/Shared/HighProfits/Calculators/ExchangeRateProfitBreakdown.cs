using System;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators
{
    using Domain.Core.Trading.Interfaces;

    public class ExchangeRateProfitBreakdown : IExchangeRateProfitBreakdown
    {
        public ExchangeRateProfitBreakdown(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            decimal positionCostWer,
            decimal positionRevenueWer,
            Domain.Core.Financial.Money.Currency fixedCurrency,
            Domain.Core.Financial.Money.Currency variableCurrency)
        {
            this.PositionCost = positionCost;
            this.PositionRevenue = positionRevenue;
            this.PositionCostWer = positionCostWer;
            this.PositionRevenueWer = positionRevenueWer;
            this.FixedCurrency = fixedCurrency;
            this.VariableCurrency = variableCurrency;
        }

        public Domain.Core.Financial.Money.Currency FixedCurrency { get; }

        public ITradePosition PositionCost { get; }

        public decimal PositionCostWer { get; }

        public ITradePosition PositionRevenue { get; }

        public decimal PositionRevenueWer { get; }

        public Domain.Core.Financial.Money.Currency VariableCurrency { get; }

        public decimal AbsoluteAmountDueToWer()
        {
            if (this.PositionCostWer == 0) return 0;

            if (this.PositionRevenueWer == 0) return 0;

            var totalVol = this.PositionCost.TotalVolumeOrderedOrFilled();

            var adjustedCost = totalVol * this.PositionCostWer;
            var adjustedRevenue = totalVol * this.PositionRevenueWer;

            return adjustedRevenue - adjustedCost;
        }

        public decimal RelativePercentageDueToWer()
        {
            if (this.PositionCostWer == 0) return 0;

            if (this.PositionRevenueWer == 0) return 0;

            var percent = this.PositionRevenueWer / this.PositionCostWer - 1;
            var roundedPercentage = Math.Round(percent, 3, MidpointRounding.AwayFromZero);

            return roundedPercentage;
        }
    }
}