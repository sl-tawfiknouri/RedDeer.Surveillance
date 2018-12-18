using System;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;

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
            DateTime universeDateTime)
        {
            var tradingHours = TradingHoursManager.Get(mic);
            if (!tradingHours.IsValid)
            {
                Logger.LogError($"RevenueCurrencyConvertingMarkingCloseCalculator was not able to get meaningful trading hours for the mic {mic}. Unable to proceed with currency conversions.");
                return null;
            }

            return new MarketDataRequest(
                mic,
                identifiers,
                tradingHours.ClosingInUtcForDay(universeDateTime).Subtract(TimeSpan.FromMinutes(15)),
                tradingHours.ClosingInUtcForDay(universeDateTime));
        }

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
