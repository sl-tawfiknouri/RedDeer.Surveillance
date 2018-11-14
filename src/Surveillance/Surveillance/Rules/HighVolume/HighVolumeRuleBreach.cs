﻿using System;
using Domain.Equity;
using Domain.Finance;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighVolume
{
    public class HighVolumeRuleBreach : IHighVolumeRuleBreach
    {
        public HighVolumeRuleBreach(
            TimeSpan window,
            ITradePosition trades,
            Security security,
            IHighVolumeRuleParameters parameters,
            BreachDetails dailyBreach,
            BreachDetails windowBreach,
            BreachDetails marketCapBreach,
            int totalOrdersTradedInWindow)
        {
            Window = window;
            Trades = trades;
            Security = security;
            Parameters = parameters;

            DailyBreach = dailyBreach;
            WindowBreach = windowBreach;
            MarketCapBreach = marketCapBreach;

            TotalOrdersTradedInWindow = totalOrdersTradedInWindow;
        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public Security Security { get; }

        public IHighVolumeRuleParameters Parameters { get; }

        public BreachDetails DailyBreach { get; }
        public BreachDetails WindowBreach { get; }
        public BreachDetails MarketCapBreach { get; }

        public int TotalOrdersTradedInWindow { get; }

        public class BreachDetails
        {
            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                int breachThresholdAmount)
            {
                HasBreach = hasBreach;
                BreachPercentage = breachPercentage;
                BreachThresholdAmount = breachThresholdAmount;
            }

            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                CurrencyAmount breachThresholdAmountCurrency)
            {
                HasBreach = hasBreach;
                BreachPercentage = breachPercentage;
                BreachThresholdAmountCurrency = breachThresholdAmountCurrency;
            }

            public bool HasBreach { get; }
            public decimal? BreachPercentage { get; }
            public int BreachThresholdAmount { get; }
            public CurrencyAmount BreachThresholdAmountCurrency { get; }

            public static BreachDetails None()
            {
                return new BreachDetails(false, null, 0);
            }
        }
    }
}