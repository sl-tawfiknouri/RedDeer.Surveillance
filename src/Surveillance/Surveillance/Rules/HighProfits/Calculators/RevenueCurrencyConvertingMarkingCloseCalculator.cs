using System;
using DomainV2.Financial;
using DomainV2.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class RevenueCurrencyConvertingMarkingCloseCalculator : RevenueCurrencyConvertingCalculator
    {
        public RevenueCurrencyConvertingMarkingCloseCalculator(
            DomainV2.Financial.Currency targetCurrency,
            ICurrencyConverter currencyConverter,
            IMarketTradingHoursManager tradingHoursManager,
            ILogger<RevenueCurrencyConvertingCalculator> logger)
            : base(
                targetCurrency,
                currencyConverter,
                tradingHoursManager,
                logger)
        { }

        protected override MarketDataRequest MarketDataRequest(
            string mic,
            InstrumentIdentifiers identifiers,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx)
        {
            var tradingHours = TradingHoursManager.Get(mic);
            if (!tradingHours.IsValid)
            {
                Logger.LogError($"RevenueCurrencyConvertingMarkingCloseCalculator was not able to get meaningful trading hours for the mic {mic}. Unable to proceed with currency conversions.");
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
