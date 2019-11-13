using System;
using Domain.Core.Financial.Assets;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators
{
    public class RevenueMarkingCloseCalculator : RevenueCalculator
    {
        public RevenueMarkingCloseCalculator(
            IMarketTradingHoursService tradingHoursService,
            ICurrencyConverterService currencyConverterService,
            ILogger<RevenueCalculator> logger)
            : base(tradingHoursService, currencyConverterService, logger)
        {
        }

        protected override MarketDataRequest MarketDataRequest(
            string marketIdentifierCode,
            InstrumentIdentifiers identifiers,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext context,
            DataSource dataSource)
        {
            var tradingHours = this.TradingHoursService.GetTradingHoursForMic(marketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.Logger.LogError(
                    $"RevenueMarkingCloseCalculator was not able to get meaningful trading hours for the mic {marketIdentifierCode}. Unable to proceed with currency conversions.");
                return null;
            }

            return new MarketDataRequest(
                marketIdentifierCode,
                string.Empty,
                identifiers,
                tradingHours.ClosingInUtcForDay(universeDateTime).Subtract(TimeSpan.FromMinutes(15)),
                tradingHours.ClosingInUtcForDay(universeDateTime),
                context?.Id(),
                dataSource);
        }
    }
}