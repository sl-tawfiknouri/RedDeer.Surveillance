using System;
using Domain.Core.Financial;
using Domain.Core.Financial.Assets;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    public class RevenueCurrencyConvertingMarkingCloseCalculator : RevenueCurrencyConvertingCalculator
    {
        public RevenueCurrencyConvertingMarkingCloseCalculator(
            Domain.Core.Financial.Money.Currency targetCurrency,
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
            var tradingHours = TradingHoursManager.GetTradingHoursForMic(mic);
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
