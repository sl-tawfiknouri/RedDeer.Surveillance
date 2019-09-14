using System;
using Domain.Core.Financial.Assets;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators
{
    public class RevenueCurrencyConvertingMarkingCloseCalculator : RevenueCurrencyConvertingCalculator
    {
        public RevenueCurrencyConvertingMarkingCloseCalculator(
            Domain.Core.Financial.Money.Currency targetCurrency,
            ICurrencyConverterService currencyConverterService,
            IMarketTradingHoursService tradingHoursService,
            ILogger<RevenueCurrencyConvertingCalculator> logger)
            : base(targetCurrency, currencyConverterService, tradingHoursService, logger)
        {
        }

        protected override MarketDataRequest MarketDataRequest(
            string mic,
            InstrumentIdentifiers identifiers,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            DataSource dataSource)
        {
            var tradingHours = this.TradingHoursService.GetTradingHoursForMic(mic);
            if (!tradingHours.IsValid)
            {
                this.Logger.LogError(
                    $"RevenueCurrencyConvertingMarkingCloseCalculator was not able to get meaningful trading hours for the mic {mic}. Unable to proceed with currency conversions.");
                return null;
            }

            return new MarketDataRequest(
                mic,
                string.Empty,
                identifiers,
                tradingHours.ClosingInUtcForDay(universeDateTime).Subtract(TimeSpan.FromMinutes(15)),
                tradingHours.ClosingInUtcForDay(universeDateTime),
                ctx?.Id(),
                dataSource);
        }
    }
}