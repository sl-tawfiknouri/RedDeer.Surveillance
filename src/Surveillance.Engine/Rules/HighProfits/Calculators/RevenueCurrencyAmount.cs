﻿using Domain.Financial;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Calculators
{
    public class RevenueCurrencyAmount
    {
        public RevenueCurrencyAmount(
            bool hadMissingMarketData,
            CurrencyAmount? currencyAmount)
        {
            HadMissingMarketData = hadMissingMarketData;
            CurrencyAmount = currencyAmount;
        }

        public bool HadMissingMarketData { get; }
        public CurrencyAmount? CurrencyAmount { get; }
    }
}
