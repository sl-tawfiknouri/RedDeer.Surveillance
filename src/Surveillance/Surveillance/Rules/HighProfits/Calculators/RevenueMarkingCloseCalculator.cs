﻿using System;
using DomainV2.Financial;
using DomainV2.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.Markets.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class RevenueMarkingCloseCalculator : RevenueCalculator
    {
        public RevenueMarkingCloseCalculator(
            IMarketTradingHoursManager tradingHoursManager,
            ILogger<RevenueCalculator> logger)
            : base(tradingHoursManager, logger)
        { }

        protected override MarketDataRequest MarketDataRequest(string mic, InstrumentIdentifiers identifiers, DateTime universeDateTime, ISystemProcessOperationRunRuleContext ctx)
        {
            var tradingHours = TradingHoursManager.GetTradingHoursForMic(mic);
            if (!tradingHours.IsValid)
            {
                Logger.LogError($"RevenueMarkingCloseCalculator was not able to get meaningful trading hours for the mic {mic}. Unable to proceed with currency conversions.");
                return null;
            }

            return new MarketDataRequest(
                mic,
                string.Empty,
                identifiers,
                tradingHours.ClosingInUtcForDay(universeDateTime).Subtract(TimeSpan.FromMinutes(15)),
                tradingHours.ClosingInUtcForDay(universeDateTime),
                ctx?.Id());
        }
    }
}
