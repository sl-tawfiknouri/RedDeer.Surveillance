﻿using System;
using Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Calculators
{
    public class ExchangeRateProfitBreakdown : IExchangeRateProfitBreakdown
    {
        public ExchangeRateProfitBreakdown(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            decimal positionCostWer,
            decimal positionRevenueWer,
            Domain.Financial.Currency fixedCurrency,
            Domain.Financial.Currency variableCurrency)
        {
            PositionCost = positionCost;
            PositionRevenue = positionRevenue;
            PositionCostWer = positionCostWer;
            PositionRevenueWer = positionRevenueWer;
            FixedCurrency = fixedCurrency;
            VariableCurrency = variableCurrency;
        }

        public ITradePosition PositionCost { get; }
        public ITradePosition PositionRevenue { get; }

        public decimal PositionCostWer { get; }
        public decimal PositionRevenueWer { get; }

        public Domain.Financial.Currency FixedCurrency { get; }
        public Domain.Financial.Currency VariableCurrency { get; }

        public decimal RelativePercentageDueToWer()
        {
            if (PositionCostWer == 0)
            {
                return 0;
            }

            if (PositionRevenueWer == 0)
            {
                return 0;
            }

            var percent = (PositionRevenueWer / PositionCostWer) -1;
            var roundedPercentage = Math.Round(percent, 3, MidpointRounding.AwayFromZero);

            return roundedPercentage;
        }

        public decimal AbsoluteAmountDueToWer()
        {
            if (PositionCostWer == 0)
            {
                return 0;
            }

            if (PositionRevenueWer == 0)
            {
                return 0;
            }

            var totalVol = PositionCost.TotalVolumeOrderedOrFilled();

            var adjustedCost = totalVol * PositionCostWer;
            var adjustedRevenue = totalVol * PositionRevenueWer;

            return adjustedRevenue - adjustedCost;
        }
    }
}