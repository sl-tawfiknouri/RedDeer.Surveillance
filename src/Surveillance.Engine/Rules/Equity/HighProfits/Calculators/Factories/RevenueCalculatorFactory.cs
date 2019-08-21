namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories
{
    using System;

    using Domain.Core.Financial.Money;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

    public class RevenueCalculatorFactory : IRevenueCalculatorFactory
    {
        private readonly ICurrencyConverterService _currencyConverterService;

        private readonly ILogger<RevenueCurrencyConvertingCalculator> _currencyConvertingLogger;

        private readonly ILogger<RevenueCalculator> _logger;

        private readonly IMarketTradingHoursService _tradingHoursService;

        public RevenueCalculatorFactory(
            IMarketTradingHoursService tradingHoursService,
            ICurrencyConverterService currencyConverterService,
            ILogger<RevenueCurrencyConvertingCalculator> currencyConvertingLogger,
            ILogger<RevenueCalculator> logger)
        {
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._currencyConverterService = currencyConverterService
                                             ?? throw new ArgumentNullException(nameof(currencyConverterService));
            this._currencyConvertingLogger = currencyConvertingLogger
                                             ?? throw new ArgumentNullException(nameof(currencyConvertingLogger));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IRevenueCalculator RevenueCalculator()
        {
            return new RevenueCalculator(this._tradingHoursService, this._logger);
        }

        public IRevenueCalculator RevenueCalculatorMarketClosureCalculator()
        {
            return new RevenueMarkingCloseCalculator(this._tradingHoursService, this._logger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingCalculator(Currency currency)
        {
            return new RevenueCurrencyConvertingCalculator(
                currency,
                this._currencyConverterService,
                this._tradingHoursService,
                this._currencyConvertingLogger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(Currency currency)
        {
            return new RevenueCurrencyConvertingMarkingCloseCalculator(
                currency,
                this._currencyConverterService,
                this._tradingHoursService,
                this._currencyConvertingLogger);
        }
    }
}