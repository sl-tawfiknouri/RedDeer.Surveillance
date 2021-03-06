﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators
{
    using Domain.Core.Trading.Interfaces;

    public class ExchangeRateProfitCalculator : IExchangeRateProfitCalculator
    {
        private readonly ILogger<ExchangeRateProfitCalculator> _logger;

        private readonly ITradePositionWeightedAverageExchangeRateService _werExchangeRateService;

        public ExchangeRateProfitCalculator(
            ITradePositionWeightedAverageExchangeRateService werExchangeRateService,
            ILogger<ExchangeRateProfitCalculator> logger)
        {
            this._werExchangeRateService =
                werExchangeRateService ?? throw new ArgumentNullException(nameof(werExchangeRateService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ExchangeRateProfitBreakdown> ExchangeRateMovement(
            ITradePosition positionCost,
            ITradePosition positionRevenue,
            Domain.Core.Financial.Money.Currency variableCurrency,
            ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (string.IsNullOrEmpty(variableCurrency.Code))
            {
                this._logger.LogInformation(
                    "ExchangeRateProfitCalculator ExchangeRateMovement had a null or empty variable currency. Returning null.");
                return null;
            }

            var orderCurrency = positionCost.Get()
                .FirstOrDefault(pos => !string.IsNullOrWhiteSpace(pos.OrderCurrency.Code))?.OrderCurrency;

            if (string.Equals(orderCurrency?.Code, variableCurrency.Code, StringComparison.InvariantCultureIgnoreCase))
            {
                this._logger.LogInformation(
                    "ExchangeRateProfitCalculator ExchangeRateMovement could not find an order currency. Returning null.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(orderCurrency.GetValueOrDefault().Code))
                orderCurrency = positionRevenue.Get()
                    .FirstOrDefault(pos => !string.IsNullOrWhiteSpace(pos.OrderCurrency.Code))?.OrderCurrency;

            var costRates =
                await this._werExchangeRateService.WeightedExchangeRate(positionCost, variableCurrency, ruleCtx);
            var revenueRates = await this._werExchangeRateService.WeightedExchangeRate(
                                   positionRevenue,
                                   variableCurrency,
                                   ruleCtx);

            var breakdown = new ExchangeRateProfitBreakdown(
                positionCost,
                positionRevenue,
                costRates,
                revenueRates,
                orderCurrency.GetValueOrDefault(),
                variableCurrency);

            return breakdown;
        }
    }
}